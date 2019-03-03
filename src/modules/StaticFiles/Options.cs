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

        public class FolderOption : StaticFilesFolderOption
        {
            public String[] DefaultFiles { get; set; }
            public bool EnableDirectoryBrowser { get; set; } = false;
        }

        public class StaticFilesFolderOption
        {
            public string Path { get; set; }
            public string RequestPath { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public Dictionary<string, string> MIMEtypes { get; set; }
            private bool? _isRelativePath;
            public bool IsRelativePath { get {
                    if (_isRelativePath == null)
                        _isRelativePath = ((Path != null && System.Text.RegularExpressions.Regex.IsMatch(Path, @"^([a-z]:)*(\/*(\.*[a-z0-9]+\/)*(\.*[a-z0-9]+))")));

                    return _isRelativePath.Value;
                } set { _isRelativePath = value; } }
            public string Root(string basePath) => 
                IsRelativePath
                ? 
                System.IO.Path.Combine(basePath, Path)
                : 
                Path;
            public IFileProvider FileProvider(string basePath) => new PhysicalFileProvider(Root(basePath));
        }
    }
}