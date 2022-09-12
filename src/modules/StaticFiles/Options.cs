using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.StaticFiles;

public class Options : IOptions
{
    public FolderOption[] Paths { get; set; } = Array.Empty<FolderOption>();

    public class FolderOption : Ws.Core.Shared.StaticFilesFolder.Options
    {
        [Description("List of default static files, i.e. index.html")]
        public string[] DefaultFiles { get; set; } = Array.Empty<string>();
        [DefaultValue(false)]
        public bool EnableDirectoryBrowser { get; set; } = false;
    }
}