using System;
using System.Collections.Generic;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.HealthCheck
{
    public class AuthOptions
    {
        public IEnumerable<string> AuthPolicies { get; set; }
        public IEnumerable<string> AuthHosts { get; set; }
    }
    public class Options: IOptions
    {
        public IEnumerable<Route> Routes { get; set; }
            = new List<Route> {
                new Route { Path = "/healtz", ContentType = RouteContentType.text, SkipChecks = true },
                new Route { Path = "/healtz/checks", ContentType = RouteContentType.json, SkipChecks = false }
            };
        public CheckEntries Checks { get; set; }
        public UiOptions Ui { get; set; } = new UiOptions();
        public class Route : AuthOptions
        {
            public string Path { get; set; }
            public RouteContentType ContentType { get; set; } = RouteContentType.json;
            /// <summary>
            /// If true skip any defined checks, returning a basic health check endpoint
            /// </summary>
            public bool SkipChecks { get; set; } = false;
        }
        public enum RouteContentType
        {
            json,
            text
        }
        public class CheckEntries
        {
            public IEnumerable<StorageCheck> Storage { get; set; }
            public MemoryCheck Memory { get; set; }
            public IEnumerable<ProcessCheck> Process { get; set; }
            public IEnumerable<WinServiceCheck> WinService { get; set; }
            public IEnumerable<TcpCheck> Tcp { get; set; }
            public IEnumerable<HttpCheck> Http { get; set; }
        }
        public class ProcessCheck: HealthResult
        {
            public string ProcessName { get; set; }
        }
        public class WinServiceCheck : HealthResult
        {
            public string ServiceName { get; set; }
        }
        public class TcpCheck : HealthResult
        {
            public string Host { get; set; }
            public int Port { get; set; }
        }
        public class MemoryCheck: HealthResult
        {
            public int MaximumAllocatedMb { get; set; }
        }
        public class HealthResult
        {
            public string Name { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
            public Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus Status { get; set; } = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy;
        }

        public class HttpCheck : HealthResult
        {
            public string Url { get; set; }
        }
        public class StorageCheck: HealthResult
        {
         /// <summary>
         /// i.e. C:\
         /// </summary>
            public string Driver { get; set; }
            public long MinimumFreeMb { get; set; }
        }

        /// <summary>
        /// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.UI/Configuration/Options.cs
        /// </summary>
        public class UiOptions: AuthOptions
        {
            public bool Enabled { get; set; } = false;
            public string Route { get; set; } = "/healthchecks-ui";
            public string RouteApi { get; set; } = "/healthchecks-api";
            public string RouteWebhook { get; set; } = "/healthchecks-webhooks";
            /// <summary>
            /// i.e. wwwroot/healthcheck-ui/style.css
            /// </summary>
            public string InjectCss { get; set; }
            public IEnumerable<EndpointOptions> Endpoints { get; set; }
            public IEnumerable<WebhookOptions> Webhooks { get; set; }
            public int EvaluationTimeinSeconds { get; set; } = 60;
            public int MinimumSecondsBetweenFailureNotifications { get; set; } = 300;
            public class EndpointOptions
            {
                public string Name { get; set; }
                public string Uri { get; set; }
            }
            public class WebhookOptions: EndpointOptions
            {
                public string Payload { get; set; }
                public string RestorePayload { get; set; }
            }
        }
    }
}
