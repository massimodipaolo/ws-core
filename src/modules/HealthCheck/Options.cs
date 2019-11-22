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
    public class Options: AuthOptions,IOptions
    { 
        public string Route { get; set; } = "/healtz";
        public CheckEntries Checks { get; set; }
        public UiOptions Ui { get; set; } = new UiOptions();
        public class CheckEntries
        {
            public IEnumerable<TcpOptions> Tcp { get; set; }
        }
        public class TcpOptions
        {
            public string Name { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }

        /// <summary>
        /// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/samples/HealthChecks.UI.Sample/appsettings.json
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
            public int MinimumSecondsBetweenFailureNotifications { get; set; } = 180;
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
