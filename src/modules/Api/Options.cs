using core.Extensions.Base;
using System.Collections.Generic;

namespace core.Extensions.Api
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
            public IEnumerable<EndpointOptions> Endpoints { get; set; }
            public XmlCommentsOptions XmlComments { get; set; }
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
        }

    }
}
