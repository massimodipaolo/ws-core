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
        private IEnumerable<Options.PolicyOption>? namedPolicies => options?.Policies?.Where(_ => !string.IsNullOrEmpty(_.Name));

        public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            if (namedPolicies?.Any() == true)
                builder.Services.AddCors(opt =>
                {
                    foreach (var policy in namedPolicies.Where(_ => _.Origins?.Any(__ => !string.IsNullOrEmpty(__)) == true))
                        _addPolicy(opt, policy);
                });
            else
                builder.Services.AddCors();

        }

        public override void Execute(WebApplication app)
        {
            base.Execute(app);

            if (namedPolicies?.Any() == true)
            {
                if (namedPolicies?.Count() == 1)
                    app.UseCors(namedPolicies?.FirstOrDefault()?.Name ?? "");
                else
                {   // https://stackoverflow.com/a/59662110
                    IDictionary<string, string> customPolicies = new Dictionary<string, string>();
                    foreach (var p in namedPolicies ?? Array.Empty<Options.PolicyOption>())
                        foreach (var o in p.Origins ?? Array.Empty<string>())
                            if (!customPolicies.ContainsKey(o))
                                customPolicies.Add(o, p.Name ?? "");
                    app.UseMiddleware<Cors.MultiOriginMiddleware>(customPolicies);
                }
            }
            else // deny all
                app.UseCors(_ => _.SetIsOriginAllowed(_ => false));
        }


        private static void _addPolicy(Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions options, Options.PolicyOption policy)
        {
            options.AddPolicy(policy.Name ?? "", _ =>
            {
                _.WithOrigins((policy.Origins ?? Array.Empty<string>()).Distinct().ToArray());

                var _methods = policy.Methods?.Where(__ => !string.IsNullOrEmpty(__));
                if (_methods?.Any() == true) { _.WithMethods(_methods.Distinct().ToArray()); } else { _.AllowAnyMethod(); }

                var _headers = policy.Headers?.Where(__ => !string.IsNullOrEmpty(__));
                if (_headers?.Any() == true) { _.WithHeaders(_headers.Distinct().ToArray()); } else { _.AllowAnyHeader(); }

                var _exposedHeaders = policy.ExposedHeaders?.Where(__ => !string.IsNullOrEmpty(__));
                if (_exposedHeaders?.Any() == true) { _.WithExposedHeaders(_exposedHeaders.Distinct().ToArray()); }

                if (policy.AllowCredentials == true) { _.AllowCredentials(); } else { _.DisallowCredentials(); }

                if (policy.PreflightMaxAgeInSeconds != null) _.SetPreflightMaxAge(TimeSpan.FromSeconds(policy.PreflightMaxAgeInSeconds.Value));

            });
        }

    }

}

