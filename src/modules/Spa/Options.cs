 using Ws.Core.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

namespace Ws.Core.Extensions.Spa
{
    public class Options : IOptions
    {
        private const string _clientFolder = "client";
        [Description("Path relative to the application root, of the directory in which the physical files are located. If the specified directory does not exist, then the SpaStaticFiles middleware will not serve any static files.")]
        [DefaultValue("client/dist")]
        public string RootPath { get; set; } = $"{_clientFolder}/dist";
        [Description("Default page that hosts your SPA user interface")]
        [DefaultValue("/index.html")]
        public string DefaultPage { get; set; } = "/index.html";
        [Description("Path, relative to the application working directory, of the directory that contains the SPA source files during development.The directory may not exist in published applications")]
        [DefaultValue("client")]
        public string SourcePath { get; set; } = _clientFolder;
        [Description("Maximum duration that a request will wait for the SPA to become ready to serve to the client")]
        [DefaultValue(90)]
        public int StartupTimeoutInSeconds { get; set; } = 90;
        [Description("Use only in development! Forward incoming requests to a local development server,i.e. http://localhost:3000.")]
        public string SpaDevelopmentServer { get; set; } // http://localhost:3000
        [Description("Use only in development! The name of the script in your package.json file that launches the React CLI process, i.e. start. This handles requests by passing them through to an instance of the React CLI server; alternative to spaDevelopmentServer")]
        public string CliServerScript { get; set; } // dev
        public Ws.Core.Shared.StaticFilesFolder.Options[] StaticFilesPaths { get; set; }
        [Description("Cache prerendering output, injecting app ICache implementation")]
        public CacheResponseOptions CacheResponse { get; set; }
        public class CacheResponseOptions
        {
            [DefaultValue(true)]
            public bool Enable { get; set; } = true;
            /// <summary>
            /// Add Link header with resources to preload
            /// </summary>
            [Description("Browsers that support preload will initiate earlier fetch of page resources: https://www.w3.org/TR/preload")]
            public EarlyHintsOptions AddEarlyHints { get; set; }
            /// <summary>
            /// Don't cache parameterized path
            /// <see cref="T:core.Extensions.Spa.Options.PrerenderingOptions.CacheResponseOptions"/> skip query
            /// string path.
            /// </summary>
            /// <value><c>true</c> if skip query string path; otherwise, <c>false</c>.</value>
            [Description("If true, don't cache parameterized path")]
            [DefaultValue(true)]
            public bool SkipQueryStringPath { get; set; } = true;
            /// <summary>
            /// Limit to extensionless path
            /// <see cref="T:core.Extensions.Spa.Options.PrerenderingOptions.CacheResponseOptions"/> skip file path.
            /// </summary>
            /// <value><c>true</c> if skip file path; otherwise, <c>false</c>.</value>
            [Description("If true, cache only extensionless path")]
            [DefaultValue(true)]
            public bool SkipFilePath { get; set; } = true;
            /// <summary>
            /// Gets or sets an array of path prefixes for which cache is disabled.
            /// </summary>
            /// <value>The exclude paths.</value>
            [Description("Array of path prefixes for which cache is disabled")]
            public string[] ExcludePaths { get; set; }
            /// <summary>
            /// Gets or sets an array of path prefixes for which cache is always enabled.
            /// </summary>
            /// <value>The include paths.</value>
            [Description("Array of path prefixes for which cache is always enabled")]
            public string[] IncludePaths { get; set; }

            public class EarlyHintsOptions
            {
                [DefaultValue(false)]
                public bool Enable { get; set; } = false;
                [Description("\"script\", \"style\", \"image\": https://www.w3.org/TR/preload/#as-attribute")]
                public string[] Types { get; set; } = new string[] { "style", "script" };
                [DefaultValue(10)]
                public int MaxItemsPerType { get; set; } = 10;
                [Description("https://www.w3.org/TR/preload/#server-push-http-2")]
                [DefaultValue(true)]
                public bool AllowServerPush { get; set; } = true;
            }
        }

    }
}
