using System;
using System.Collections.Generic;
using System.Linq;
using core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace core.Extensions.Api
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();
        public override void Execute(IServiceCollection services, IServiceProvider serviceProvider)
        {
            base.Execute(services, serviceProvider);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var _session = _options.Session;
            if (_session != null)
            {
                services.AddSession(opt =>
                {
                    var _cookie = _session.Cookie;
                    if (_cookie != null)
                    {
                        opt.Cookie.Name = _cookie.Name;
                        if (!string.IsNullOrEmpty(_cookie.Path))
                            opt.Cookie.Path = _cookie.Path;
                        if (!string.IsNullOrEmpty(_cookie.Domain))
                            opt.Cookie.Domain = _cookie.Domain;
                        opt.Cookie.HttpOnly = _cookie.HttpOnly;
                    }
                    opt.IdleTimeout = TimeSpan.FromMinutes(_session.IdleTimeoutInMinutes);
                });
            }

            var _serialization = _options.Serialization ?? new Options.SerializationOptions();
            services
                .AddMvc()
                .AddJsonOptions(opt => {
                    var _setting = opt.SerializerSettings;                    
                    _setting.NullValueHandling = _serialization.NullValueHandling;
                    _setting.Formatting = _serialization.Formatting;
                    _setting.ReferenceLoopHandling = _serialization.ReferenceLoopHandling;
                });

            var _doc = _options.Documentation;
            if (_doc != null)
            {
                services.AddSwaggerGen(opt =>
                {
                    foreach (var version in _doc.Versions.Select((v, i) => new { v, i }))
                    {
                        var _version = string.IsNullOrEmpty(version.v.Id) ? $"v{version.i + 1}" : version.v.Id;
                        opt.SwaggerDoc(
                            _version,
                            new Swashbuckle.AspNetCore.Swagger.Info()
                            {
                                Title = string.IsNullOrEmpty(version.v.Title) ? $"API v{version.i + 1}" : version.v.Title,
                                Version = _version
                            }
                            );
                    }

                    //Xml comments
                    if (_doc.XmlComments != null && string.IsNullOrEmpty(_doc.XmlComments.FileName))
                    {
                        var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, _doc.XmlComments.FileName);                        
                        opt.IncludeXmlComments(filePath, includeControllerXmlComments: _doc.XmlComments.IncludeControllerComments);
                    }

                });
            }
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            if (_options.Session != null)
                applicationBuilder.UseSession();

            applicationBuilder.UseMvcWithDefaultRoute();

            var _doc = _options.Documentation;
            if (_doc != null)
            {
                applicationBuilder.UseSwagger(opt =>
                {
                    opt.RouteTemplate = _doc.RoutePrefix + "/{documentName}/swagger.json";
                    /*opt.PreSerializeFilters.Add((doc, rq) =>
                    {
                        doc.Host = rq.Host.Value;
                    });*/
                });

                applicationBuilder.UseSwaggerUI(opt =>
                {
                    opt.RoutePrefix = _doc.RoutePrefix;
                    foreach (var version in _doc.Versions.Select((v, i) => new { v, i }))
                    {
                        var _version = string.IsNullOrEmpty(version.v.Id) ? $"v{version.i + 1}" : version.v.Id;
                        opt.SwaggerEndpoint($"/{_doc.RoutePrefix}/{_version}/swagger.json", string.IsNullOrEmpty(version.v.Title) ? $"API v{version.i + 1}" : version.v.Title);                        
                    }
                });
            }
        }
    }
}
