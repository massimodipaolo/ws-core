using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Base
{
    public partial class Options
    {
        public StaticFileOptions StaticFiles { get; set; }
        public class StaticFileOptions
        {
            public string Path { get; set; }
            public string RequestPath { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public Dictionary<string, string> MIMEtypes { get; set; }
            public String[] DefaultFiles { get; set; }
            public bool EnableDirectoryBrowser { get; set; } = false;
        }
    }
}