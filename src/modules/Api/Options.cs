using Ws.Core.Extensions.Base;
using System.Collections.Generic;

namespace Ws.Core.Extensions.Api
{
    public class Options : IOptions
    {
        public SerializationOptions Serialization { get; set; }
        public SessionOptions Session { get; set; }
        public DocumentationOptions Documentation { get; set; }


        public class SerializationOptions
        {
            public Newtonsoft.Json.NullValueHandling NullValueHandling { get; set; } = Newtonsoft.Json.NullValueHandling.Ignore;
            public Newtonsoft.Json.Formatting Formatting { get; set; } = Newtonsoft.Json.Formatting.None;
            public Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling { get; set; } = Newtonsoft.Json.ReferenceLoopHandling.Error;
            public Newtonsoft.Json.DateParseHandling DateParseHandling { get; set; } = Newtonsoft.Json.DateParseHandling.DateTime;
            public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling { get; set; } = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
        }

        public class SessionOptions
        {
            public CookieOptions Cookie { get; set; }
            public int IdleTimeoutInMinutes { get; set; } = 20;

            public class CookieOptions
            {
                public string Name { get; set; } = ".api.Session";
                public string Path { get; set; }
                public string Domain { get; set; }
                public bool HttpOnly { get; set; } = true;
            }
        }

        public class DocumentationOptions
        {
            public string RoutePrefix { get; set; } = "swagger";
            public UiOptions Ui { get; set; }
            public IEnumerable<EndpointOptions> Endpoints { get; set; }
            public XmlCommentsOptions XmlComments { get; set; }
            public SecurityDefinitionsOptions SecurityDefinitions { get; set; }
            public class UiOptions
            {
                public string InjectJs { get; set; }
                public string InjectCss { get; set; }
            }
            public class EndpointOptions
            {
                public string Id { get; set; }
                public string Title { get; set; }
                public string Version { get; set; }
            }
            public class XmlCommentsOptions {
                public string FileName { get; set; }
                public bool IncludeControllerComments { get; set; } = false;
            }
            public class SecurityDefinitionsOptions
            {
                public bool Bearer { get; set; } = false;
                public string[] Cookies { get; set; }
            }
        }
        

    }
}
