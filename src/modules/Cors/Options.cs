using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Cors
{
    public class Options : IOptions
    {
        public PolicyOption[]? Policies { get; set; }

        public class PolicyOption
        {
            [Required]
            public string Name { get; set; }
            [Description("To allow one or more specific origins")]
            public string[]? Origins { get; set; }
            [Description("GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS,TRACE")]
            [EnumDataType(typeof(HttpMethod))]
            public string[]? Methods { get; set; }
            [Description("To whitelist specific headers")]
            public string[]? Headers { get; set; }
            [Description("The CORS spec calls simple response headers. Specify other headers available to the application")]
            public string[]? ExposedHeaders { get; set; }
            [Description("Credentials include cookies as well as HTTP authentication schemes")]
            [DefaultValue(false)]
            public bool? AllowCredentials { get; set; } = false;
            [Description("Value in seconds. The Access-Control-Max-Age header specifies how long the response to the preflight request can be cached")]
            public int? PreflightMaxAgeInSeconds { get; set; }

            public enum HttpMethod
            {
                GET,HEAD,POST,PUT,PATCH,DELETE,OPTIONS,TRACE
            }
        }
    }
}
