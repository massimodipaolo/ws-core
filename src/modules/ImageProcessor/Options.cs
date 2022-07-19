using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.ImageProcessor;

public class Options : IOptions
{
    [Description("ImageSharp.Web config: https://docs.sixlabors.com/articles/imagesharp.web/gettingstarted.html")]
    public SixLabors.ImageSharp.Web.Middleware.ImageSharpMiddlewareOptions? Config { get; set; } = new ();
    public SixLabors.ImageSharp.Web.Caching.PhysicalFileSystemCacheOptions? FileSystemCache { get; set; } = new(); 
    
    public Options() {
    }

}
