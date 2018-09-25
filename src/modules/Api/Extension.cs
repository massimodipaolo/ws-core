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
                .AddJsonOptions(opt =>
                {
                    var _setting = opt.SerializerSettings;
                    _setting.NullValueHandling = _serialization.NullValueHandling;
                    _setting.Formatting = _serialization.Formatting;
                    _setting.ReferenceLoopHandling = _serialization.ReferenceLoopHandling;
                    _setting.DateParseHandling = _serialization.DateParseHandling;
                    _setting.DateTimeZoneHandling = _serialization.DateTimeZoneHandling;
                });

            var _doc = _options.Documentation;
            if (_doc != null)
            {
                services.AddSwaggerGen(opt =>
                {
                    foreach (var doc in _doc.Endpoints?.Select((e, i) => new { e, i }))
                    {
                        var _id = string.IsNullOrEmpty(doc.e?.Id) ? $"v{doc.i + 1}" : doc.e?.Id;
                        opt.SwaggerDoc(
                            _id,
                            new Swashbuckle.AspNetCore.Swagger.Info()
                            {
                                Title = string.IsNullOrEmpty(doc.e?.Title) ? $"API v{doc.i + 1}" : doc.e?.Title,
                                Version = doc.e?.Version ?? _id
                            }
                            );
                    }

                    //Xml comments
                    if (_doc.XmlComments != null && !string.IsNullOrEmpty(_doc.XmlComments.FileName))
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
                    foreach (var doc in _doc.Endpoints?.Select((e, i) => new { e, i }))
                    {
                        var _id = string.IsNullOrEmpty(doc.e.Id) ? $"v{doc.i + 1}" : doc.e.Id;
                        opt.SwaggerEndpoint($"/{_doc.RoutePrefix}/{_id}/swagger.json", string.IsNullOrEmpty(doc.e.Title) ? $"API v{doc.i + 1}" : doc.e.Title);
                    }
                });
            }
        }
    }
}
