using Ws.Core.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;

namespace Ws.Core.Extensions.Spa
{
    public class Options : IOptions
    {
        private const string _clientFolder = "Client";
        public string RootPath { get; set; } = $"{_clientFolder}/dist/browser";
        public string DefaultPage { get; set; } = "/index.html";
        public string SourcePath { get; set; } = _clientFolder;
        public int StartupTimeoutInSeconds { get; set; } = 90;
        public string SpaDevelopmentServer { get; set; } // http://localhost:4200
        public string CliServerScript { get; set; } // start
        public Ws.Core.Shared.StaticFilesFolder.Options[] StaticFilesPaths { get; set; }
        public PrerenderingOptions Prerendering { get; set; }

        public class PrerenderingOptions
        {
            public bool Enable { get; set; } = true;
            public CacheResponseOptions CacheResponse { get; set; }
            public string BootModulePath { get; set; } = $"{_clientFolder}/dist/server/main.js";
            public string BootModuleBuilderScript { get; set; }
            public string[] ExcludeUrls { get; set; }
            public string[] ContextData { get; set; }

            public class CacheResponseOptions
            {
                public bool Enable { get; set; } = true;
                /// <summary>
                /// Add Link header with resources to preload
                /// </summary>
                public EarlyHintsOptions AddEarlyHints { get; set; }
                /// <summary>
                /// Don't cache parameterized path
                /// <see cref="T:core.Extensions.Spa.Options.PrerenderingOptions.CacheResponseOptions"/> skip query
                /// string path.
                /// </summary>
                /// <value><c>true</c> if skip query string path; otherwise, <c>false</c>.</value>
                public bool SkipQueryStringPath { get; set; } = true;
                /// <summary>
                /// Limit to extensionless path
                /// <see cref="T:core.Extensions.Spa.Options.PrerenderingOptions.CacheResponseOptions"/> skip file path.
                /// </summary>
                /// <value><c>true</c> if skip file path; otherwise, <c>false</c>.</value>
                public bool SkipFilePath { get; set; } = true;
                /// <summary>
                /// Gets or sets an array of path prefixes for which cache is disabled.
                /// </summary>
                /// <value>The exclude paths.</value>
                public string[] ExcludePaths { get; set; }
                /// <summary>
                /// Gets or sets an array of path prefixes for which cache is always enabled.
                /// </summary>
                /// <value>The include paths.</value>
                public string[] IncludePaths { get; set; }

                public class EarlyHintsOptions
                {
                    public bool Enable { get; set; } = false;
                    public string[] Types { get; set; }
                    public int MaxItemsPerType { get; set; } = 10;
                    public bool AllowServerPush { get; set; } = true;
                }
            }

        }

    }
}
