using Ws.Core.Extensions.Base;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ws.Core.Extensions.Api
{
    public class Options : IOptions
    {
        public Ws.Core.Shared.Serialization.Options? Serialization { get; set; } = new Ws.Core.Shared.Serialization.Options();
        public SessionOptions? Session { get; set; }
        public DocumentationOptions? Documentation { get; set; }

        public class SessionOptions
        {
            public CookieOptions Cookie { get; set; }
            [Description("The IdleTimeout indicates how long the session can be idle before its contents are abandoned. Each session access resets the timeout. Note this only applies to the content of the session, not the cookie.")]
            [DefaultValue(20)]
            public int IdleTimeoutInMinutes { get; set; } = 20;

            public class CookieOptions: CookieBuilder
            {
                [Description("The name of the cookie.")]
                [DefaultValue(".api.Session")]
                public override string Name { get; set; } = ".api.Session";
                [Description("Indicates whether a cookie is accessible by client-side script.")]
                [DefaultValue(true)]
                public override bool HttpOnly { get; set; } = true;
            }
        }

        public class DocumentationOptions
        {
            [DefaultValue("swagger")]
            public string? RoutePrefix { get; set; } = "swagger";
            public UiOptions? Ui { get; set; }
            [Required]
            public IEnumerable<EndpointOptions> Endpoints { get; set; }
            public XmlCommentsOptions? XmlComments { get; set; }
            public SecurityDefinitionsOptions? SecurityDefinitions { get; set; }
            public class UiOptions
            {
                [Description("Relative path of additional js file, added in wwwroot folder; i.e. \"/swagger-ui/custom.js\"")]
                public string InjectJs { get; set; }
                [Description("Relative path of additional css file, added in wwwroot folder; i.e. \"/swagger-ui/custom.css\"")]
                public string InjectCss { get; set; }
            }
            public class EndpointOptions
            {
                [DefaultValue("v{index}")]
                public string Id { get; set; }
                [DefaultValue("API v{index}")]
                public string Title { get; set; }
                [DefaultValue("{Id}")] 
                public string Version { get; set; }
            }
            [Description("To include Xml Comments, open the Properties dialog for your project, click the \"Build\" tab and ensure that \"XML documentation file\" is checked.\n This will produce a file containing all XML comments at build-time.\nAt this point, any classes or methods that are NOT annotated with XML comments will trigger a build warning.\n To suppress this, enter the warning code 1591 into the \"Suppress warnings\" field in the properties dialog.")]
            public class XmlCommentsOptions {
                [Description("i.e. api.xml\nCheck PropertyGroup>DocumentationFile value in your .csproj file.")]
                [DefaultValue("System.Reflection.Assembly.GetExecutingAssembly().GetName().Name")]
                public string FileName { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                [DefaultValue(false)]
                public bool IncludeControllerComments { get; set; } = false;
            }
            [Description("Add one or more security definitions, describing how your api is protected")]
            public class SecurityDefinitionsOptions
            {
                [Description("Add Authorization header for bearer token")]
                [DefaultValue(false)]
                public bool Bearer { get; set; } = false;
                
                public string[] Cookies { get; set; }
            }
        }
        

    }
}
