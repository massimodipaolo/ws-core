using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Hellang.Middleware.ProblemDetails;

namespace Ws.Core.Extensions.Api
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();
        public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            _addProblemDetails(builder, _options.ProblemDetails);
            _addSession(builder, _options.Session);
            _addControllers(builder, _options.Serialization);
            _addDocumentation(builder, _options.Documentation);
        }

        public override void Execute(WebApplication app)
        {
            base.Execute(app);

            _useProblemDetails(app);
            _useSession(app, _options.Session);
            _useControllers(app);
            _useDocumentation(app, _options.Documentation);
        }
        private static void _addProblemDetails(WebApplicationBuilder builder, ProblemDetailsOptions? options)
        {
            _ = builder.Services.AddProblemDetails(_ =>
            {
                // map each props
                /*
                if (options != null)
                    foreach (var prop in options.GetType().GetProperties())
                        if (prop.GetValue(options) != null)
                            _.GetType().GetProperty(prop.Name)?.SetValue(_, options);
                */
                // override

                _.SourceCodeLineCount = 6;
                _.IncludeExceptionDetails = (ctx, ex) => true;
                /*
                _.OnBeforeWriteDetails = (ctx, pd) =>
                {
                    if (new[] { "Local", "Development" }.Contains(env.EnvironmentName))
                        return;                    
                    pd.Extensions.Remove(Hellang.Middleware.ProblemDetails.ProblemDetailsOptions.DefaultExceptionDetailsPropertyName);
                };
                */
            });
        }
        private static void _useProblemDetails(WebApplication app)
        {
            app.UseProblemDetails();
        }
        private static void _addSession(WebApplicationBuilder builder, Options.SessionOptions? options)
        {
            if (options != null)
            {
                builder.Services.AddSession(_ =>
                {
                    var _cookie = options.Cookie;
                    if (_cookie != null)
                    {
                        _.Cookie = _cookie;
                    }
                    _.IdleTimeout = TimeSpan.FromMinutes(options.IdleTimeoutInMinutes);
                });
            }
        }
        private static void _useSession(WebApplication app, Options.SessionOptions? options)
        {
            if (options != null)
                app.UseSession();
        }
        private static void _addControllers(WebApplicationBuilder builder, Ws.Core.Shared.Serialization.Options? options)
        {
            builder.Services
                .AddControllers()
                .AddJsonOptions(_ =>
                {
                    var _setting = _.JsonSerializerOptions;
                    options?.FromJsonSerializerSettings(ref _setting);
                })
            ;
        }

        private static void _useControllers(WebApplication app)
        {
            app.MapControllers();
        }

        private static void _addDocumentation(WebApplicationBuilder builder, Options.DocumentationOptions? options)
        {
            if (options != null)
            {
                builder.Services
                    .AddEndpointsApiExplorer()
                    .AddSwaggerGen(opt =>
                    {
                        opt.CustomSchemaIds(_ => _.FullName);
                        opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                        _addDocumentationSwaggerXml(opt, options);
                        _addDocumentationSwaggerEndpoints(opt, options);
                        _addDocumentationSwaggerSecurity(opt, options);
                    });
            }
        }

        private static void _addDocumentationSwaggerXml(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions generator, Options.DocumentationOptions options)
        {
            if (!string.IsNullOrEmpty(options.XmlComments?.FileName))
            {
                var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, options.XmlComments.FileName);
                generator.IncludeXmlComments(filePath, includeControllerXmlComments: options.XmlComments.IncludeControllerComments);
            }
        }

        private static void _addDocumentationSwaggerEndpoints(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions generator, Options.DocumentationOptions options)
        {
            foreach (var doc in options.Endpoints.Select((e, i) => new { e, i }))
            {
                var _id = string.IsNullOrEmpty(doc.e?.Id) ? $"v{doc.i + 1}" : doc.e?.Id;
                generator.SwaggerDoc(
                    _id,
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = string.IsNullOrEmpty(doc.e?.Title) ? $"api v{doc.i + 1}" : doc.e?.Title,
                        Version = doc.e?.Version ?? _id
                    }
                    );
            }
        }

        private static void _addDocumentationSwaggerSecurity(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions generator, Options.DocumentationOptions options)
        {
            if (options.SecurityDefinitions?.Bearer == true)
            {
                generator.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                generator.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                                    },
                                    Array.Empty<string>()
                                }
                            });
            }

            if (options.SecurityDefinitions?.Cookies?.Any() == true)
            {
                foreach (var cookieName in options.SecurityDefinitions.Cookies)
                {
                    generator.AddSecurityDefinition(cookieName, new OpenApiSecurityScheme
                    {
                        Description = $"Cookie Auth: {cookieName}",
                        In = ParameterLocation.Cookie,
                        Name = cookieName,
                        Type = SecuritySchemeType.ApiKey
                    });
                    generator.AddSecurityRequirement(new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = cookieName }
                                    },
                                    Array.Empty<string>()
                                }
                            });
                }
            }
        }

        private static void _useDocumentation(WebApplication app, Options.DocumentationOptions? options)
        {
            if (options != null)
            {
                _useDocumentationSwagger(app, options);
                _useDocumentationSwaggerUi(app, options);
            }
        }

        private static void _useDocumentationSwagger(WebApplication app, Options.DocumentationOptions options)
        {
            app.UseSwagger(opt =>
            {
                opt.RouteTemplate = options.RoutePrefix + "/{documentName}/swagger.json";
            });
        }

        private static void _useDocumentationSwaggerUi(WebApplication app, Options.DocumentationOptions options)
        {
            app.UseSwaggerUI(opt =>
            {
                opt.RoutePrefix = options.RoutePrefix;
                opt.EnableDeepLinking();
                opt.DisplayRequestDuration();
                // inject ui
                opt.InjectJavascript(options.Ui?.InjectJs);
                opt.InjectStylesheet(options.Ui?.InjectCss);

                foreach (var doc in options.Endpoints.Select((e, i) => new { e, i }))
                {
                    var _id = doc.e?.Id ?? $"v{doc.i + 1}";
                    opt.SwaggerEndpoint($"/{options.RoutePrefix}/{_id}/swagger.json", doc.e?.Title ?? $"API v{doc.i + 1}");
                }

            });
        }

    }
}
