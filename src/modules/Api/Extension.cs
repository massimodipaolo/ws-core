using System;
using System.Collections.Generic;
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
                services.AddSession(opt => {
                    var _cookie = _session.Cookie;
                    if (_cookie != null)
                    {
                        if (string.IsNullOrEmpty(_cookie.Name))
                            opt.Cookie.Name = _cookie.Name;
                        if (string.IsNullOrEmpty(_cookie.Path))
                            opt.Cookie.Path = _cookie.Path;
                        if (string.IsNullOrEmpty(_cookie.Domain))
                            opt.Cookie.Domain = _cookie.Domain;
                        opt.Cookie.HttpOnly = _cookie.HttpOnly;
                    }
                    opt.IdleTimeout = TimeSpan.FromMinutes(_session.IdleTimeoutInMinutes);
                });
            }            

            services.AddMvc();
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            if (_options.Session != null)
                applicationBuilder.UseSession();

            applicationBuilder.UseMvcWithDefaultRoute();
        }
    }
}
