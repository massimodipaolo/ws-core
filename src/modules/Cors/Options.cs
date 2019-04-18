using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Cors
{
    public class Options : IOptions
    {
        public PolicyOption[] Policies { get; set; }

        public class PolicyOption
        {
            public string Name { get; set; }
            public string[] Origins { get; set; }
            public string[] Methods { get; set; }
            public string[] Headers { get; set; }
            public string[] ExposedHeaders { get; set; }
            public bool AllowCredentials { get; set; } = false;
            public int? PreflightMaxAgeInSeconds { get; set; }
        }
    }
}
