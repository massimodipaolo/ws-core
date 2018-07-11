using core.Extensions.Base;

namespace core.Extensions.Spa
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
        public PrerenderingOptions Prerendering { get; set; }        

        public class PrerenderingOptions
        {
            public string BootModulePath { get; set; } = $"{_clientFolder}/dist/server/main.js";
            public string BootModuleBuilderScript { get; set; }
            public string[] ExcludeUrls { get; set; }
            public string[] ContextData { get; set; }
        }

    }
}
