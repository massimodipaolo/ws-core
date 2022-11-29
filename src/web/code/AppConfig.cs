using Ws.Core.Shared.Serialization;

namespace ws.bom.oven.web.code
{
    public class AppConfig: Ws.Core.AppConfig
    {
        public CronJobConfig CronJob { get; set; } = new();
        public class CronJobConfig
        {
            public bool Enabled { get; set; } = false;
            public Dictionary<string, string> RecurringJobsCronExpression { get; set; } = new();
        }
        public interface IGatewayBase
        {
            string? Host { get; set; }
        }
        public GatewayConfig? Gateway { get; set; }
        public class GatewayConfig
        {
            public PayloadCmsConfig? PayloadCms { get; set; }
            public class BaseConfig: IGatewayBase
            {
                public string? Host { get; set; }
            }
            public class BaseWithAuthConfig : BaseConfig
            {
                public string? UserName { get; set; }
                [SensitiveData]
                public string? Password { get; set; }
            }

            public class PayloadCmsConfig : BaseWithAuthConfig
            {
                public SlugsConfig? Slugs { get; set; } = new();
                public class SlugsConfig {
                    public string? Auth { get; set; }
                    public string? Category { get; set; }
                    public string? Market { get; set; }
                    public string? Locale { get; set; }
                    public string[]? ExcludeFromStore { get; set; }
                }
            }
        }
    }
}
