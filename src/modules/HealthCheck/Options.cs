using System;
using System.Collections.Generic;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.HealthCheck
{
    public class Options: IOptions
    { 
        public string RouteName { get; set; } = "/healtz";
        public IEnumerable<string> AuthPolicies { get; set; }
        public IEnumerable<string> AuthHosts { get; set; }
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
        public class UiOptions
        {
            public bool Enabled { get; set; } = false;
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
