using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using static Ws.Core.Extensions.StaticFiles.Options;

namespace Ws.Core.Extensions.StaticFiles;

public class Extension : Base.Extension
{
    private IEnumerable<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)> Settings
    {
        get
        {
            return _getSettings();
        }
    }

    private List<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)> _getSettings()
    {
        
        var res = new List<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)>();

        foreach (var opt in _configureFolderOptions())
        {
            if (opt != null)
            {
                var staticFileOptions = opt.GetStaticFileOptions(env?.ContentRootPath ?? Directory.GetCurrentDirectory(), logger);
                res.Add((
                    staticFileOptions, 
                    _configureDirectoryBrowser(staticFileOptions,opt),
                    _configureDefaultFiles(staticFileOptions,opt)));
            }
        }

        return res;
    }
    private FolderOption[] _configureFolderOptions()
    {
        Options opts = GetOptions<Options>();
        if (opts == null || opts.Paths == null || !opts.Paths.Any())
        {
            opts = new Options()
            {
                Paths = new FolderOption[] {
                    new () {}
                }
            };
        }
        return opts.Paths;
    }
    private static DirectoryBrowserOptions _configureDirectoryBrowser(StaticFileOptions staticFileOptions, FolderOption folderOptions)
    {
        DirectoryBrowserOptions directoryBrowserOptions = null;
        if (folderOptions.EnableDirectoryBrowser)
        {
            directoryBrowserOptions = new DirectoryBrowserOptions();
            if (!string.IsNullOrEmpty(folderOptions.Path))
                directoryBrowserOptions.FileProvider = staticFileOptions.FileProvider;
            if (!string.IsNullOrEmpty(folderOptions.RequestPath))
                directoryBrowserOptions.RequestPath = staticFileOptions.RequestPath;
        }
        return directoryBrowserOptions;
    }

    private static DefaultFilesOptions _configureDefaultFiles(StaticFileOptions staticFileOptions, FolderOption folderOptions)
    {
        DefaultFilesOptions defaultFilesOptions = null;
        if (folderOptions.DefaultFiles?.Length > 0)
        {
            defaultFilesOptions = new DefaultFilesOptions();
            if (!string.IsNullOrEmpty(folderOptions.Path))
                defaultFilesOptions.FileProvider = staticFileOptions.FileProvider;
            if (!string.IsNullOrEmpty(folderOptions.RequestPath))
                defaultFilesOptions.RequestPath = staticFileOptions.RequestPath;
            defaultFilesOptions.DefaultFileNames.Clear();
            foreach (var f in folderOptions.DefaultFiles)
                defaultFilesOptions.DefaultFileNames.Add(f);
        }
        return defaultFilesOptions;
    }

    public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
    {
        base.Execute(builder, serviceProvider);
        if (Settings.Any(_ => _.DirectoryBrowserOptions != null))
            builder.Services.AddDirectoryBrowser();
    }

    public override void Execute(WebApplication app)
    {
        base.Execute(app);

        foreach (var setting in Settings)
        {
            if (setting.DefaultFilesOptions != null)
                app.UseDefaultFiles(setting.DefaultFilesOptions);

            if (setting.DirectoryBrowserOptions != null)
                app.UseDirectoryBrowser(setting.DirectoryBrowserOptions);

            app.UseStaticFiles(setting.StaticFileOptions);
        }
    }
}
