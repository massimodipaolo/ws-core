using core.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;

namespace core.Extensions.StaticFiles
{
    public class Options: IOptions
    {           
            public FolderOption[] Paths { get; set; }

        public class FolderOption
        {
            public string Path { get; set; }
            public string RequestPath { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public Dictionary<string, string> MIMEtypes { get; set; }
            public String[] DefaultFiles { get; set; }
            public bool EnableDirectoryBrowser { get; set; } = false;
            public bool? IsRelativePath { get; set; }
            public string Root(IHostingEnvironment env) => (IsRelativePath ?? ((Path != null && System.Text.RegularExpressions.Regex.IsMatch(Path, @"^([a-z]:)*(\/*(\.*[a-z0-9]+\/)*(\.*[a-z0-9]+))")))) ? System.IO.Path.Combine(env?.ContentRootPath ?? Directory.GetCurrentDirectory(), Path) : Path;
            public IFileProvider FileProvider(IHostingEnvironment env) => new PhysicalFileProvider(Root(env));
        }
    }
}