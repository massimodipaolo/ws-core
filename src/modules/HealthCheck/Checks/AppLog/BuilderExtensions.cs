using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Ws.Core.Extensions.HealthCheck.Checks.AppLog;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppLogHealthCheckBuilderExtensions
    {
        const string NAME = "app-log";
        /// <summary>
        /// Add a health check for application log.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="appLogOptions"><see cref="Ws.Core.Extensions.HealthCheck.Checks.AppLog.Options"/></param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'app-log' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddAppLog(
            this IHealthChecksBuilder builder, 
            Ws.Core.Extensions.HealthCheck.Checks.AppLog.Options appLogOptions,
            string name = default, 
            HealthStatus? failureStatus = default, 
            IEnumerable<string> tags = default, 
            TimeSpan? timeout = default) 
        {
            try
            {
                Type type = null;
                if (!string.IsNullOrEmpty(appLogOptions.AppLogService))
                    type = Type.GetType(appLogOptions.AppLogService, throwOnError: false);
                // autodiscover
                if (type == null)
                    type = Ws.Core.Extensions.Base.Util.GetAllTypesOf(typeof(IAppLogService)).FirstOrDefault();

                if (type != null)
                    builder.Services.AddTransient(typeof(IAppLogService), type);

                var appLogService = builder.Services.BuildServiceProvider().GetService<IAppLogService>();
                if (appLogService == null) return builder;

                return builder.Add(new HealthCheckRegistration(
                   name ?? NAME,
                   sp => new Ws.Core.Extensions.HealthCheck.Checks.AppLog.HealthCheck(appLogOptions, appLogService),
                   failureStatus,
                   tags,
                   timeout));
            }
            catch(Exception ex) {
                var logger = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();
                logger?.CreateLogger<HealthCheckRegistration>()?.LogError(ex, "Error adding {extension} health check", nameof(AppLogHealthCheckBuilderExtensions));
            }

            return builder;
        }
    }
}
