using System;
using System.Collections.Generic;
using System.Text;
using core.Extensions.Base;

namespace core.Extensions.StaticFiles
{
    public class Options: IOptions
    {
            public string Path { get; set; }
            public string RequestPath { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public Dictionary<string, string> MIMEtypes { get; set; }
            public String[] DefaultFiles { get; set; }
            public bool EnableDirectoryBrowser { get; set; } = false;
    }
}