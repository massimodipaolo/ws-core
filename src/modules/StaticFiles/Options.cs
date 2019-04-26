using Ws.Core.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ws.Core.Extensions.StaticFiles
{
    public class Options : IOptions
    {
        public FolderOption[] Paths { get; set; }

        public class FolderOption : Ws.Core.Shared.StaticFilesFolder.Options
        {
            public String[] DefaultFiles { get; set; }
            public bool EnableDirectoryBrowser { get; set; } = false;
        }
    }
}