using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Cors
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();
        private IEnumerable<Options.PolicyOption> namedPolicies => options?.Policies?.Where(_ => !string.IsNullOrEmpty(_.Name));

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            if (namedPolicies != null && namedPolicies.Any())  
            {
                serviceCollection.AddCors(opt =>
                {
                    foreach (var p in namedPolicies)
                    {
                        opt.AddPolicy(p.Name, _ =>
                        {
                            var _origins = p.Origins?.Where(__ => !string.IsNullOrEmpty(__));
                            if (_origins != null && _origins.Any()) { _.WithOrigins(_origins.ToArray()); } else { _.AllowAnyOrigin(); };

                            var _methods = p.Methods?.Where(__ => !string.IsNullOrEmpty(__));
                            if (_methods != null && _methods.Any()) { _.WithMethods(_methods.ToArray()); } else { _.AllowAnyMethod(); };

                            var _headers = p.Headers?.Where(__ => !string.IsNullOrEmpty(__));
                            if (_headers != null && _headers.Any()) { _.WithHeaders(_headers.ToArray()); } else { _.AllowAnyHeader(); };

                            var _exposedHeaders = p.ExposedHeaders?.Where(__ => !string.IsNullOrEmpty(__));
                            if (_exposedHeaders != null && _exposedHeaders.Any()) { _.WithExposedHeaders(_exposedHeaders.ToArray()); };

                            if (p.AllowCredentials) { _.AllowCredentials(); } else { _.DisallowCredentials(); };

                            if (p.PreflightMaxAgeInSeconds != null) _.SetPreflightMaxAge(TimeSpan.FromSeconds(p.PreflightMaxAgeInSeconds.Value));
                        });
                    }
                });
            }
            else
                serviceCollection.AddCors();

        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            if (namedPolicies != null && namedPolicies.Any())
                foreach (var p in namedPolicies)
                    applicationBuilder.UseCors(p.Name);
            else
                applicationBuilder.UseCors(_ => _
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()                
            );
        }
    }

}

