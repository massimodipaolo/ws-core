using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace core.Extensions.StaticFiles
{
    public class Extension : Base.Extension
    {
        private IEnumerable<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)> Settings
        {
            get
            {                
                IEnumerable<Options> opts = GetOptions<List<Options>>();
                var res = new List<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)>();
                foreach (var opt in opts)
                {

                    //StaticFileOptions
                    var StaticFileOptions = new StaticFileOptions();
                    if (!string.IsNullOrEmpty(opt.Path))
                        //TODO: Inject IFileProvider (or ServiceLocator serviceProvider.GetService<IFileProvider>()): https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers
                        StaticFileOptions.FileProvider = new PhysicalFileProvider(Path.Combine(ContentPath, opt.Path));
                    if (!string.IsNullOrEmpty(opt.RequestPath))
                        StaticFileOptions.RequestPath = new PathString(opt.RequestPath);
                    if (opt.Headers != null)
                    {
                        StaticFileOptions.OnPrepareResponse = ctx =>
                        {
                            foreach (var h in opt.Headers)
                                ctx.Context.Response.Headers.Append(h.Key, h.Value);
                        };
                    }
                    if (opt.MIMEtypes != null)
                    {
                        var provider = new FileExtensionContentTypeProvider();
                        foreach (var t in opt.MIMEtypes)
                            provider.Mappings[t.Key] = t.Value;
                        StaticFileOptions.ContentTypeProvider = provider;
                    }

                    //DirectoryBrowser
                    DirectoryBrowserOptions DirectoryBrowserOptions = null;
                    if (opt.EnableDirectoryBrowser)
                    {
                        DirectoryBrowserOptions = new DirectoryBrowserOptions();
                        if (!string.IsNullOrEmpty(opt.Path))
                            DirectoryBrowserOptions.FileProvider = StaticFileOptions.FileProvider;
                        if (!string.IsNullOrEmpty(opt.RequestPath))
                            DirectoryBrowserOptions.RequestPath = StaticFileOptions.RequestPath;
                    }

                    //DefaultFiles
                    DefaultFilesOptions DefaultFilesOptions = null;
                    if (opt.DefaultFiles?.Length > 0)
                    {
                        DefaultFilesOptions = new DefaultFilesOptions();
                        if (!string.IsNullOrEmpty(opt.Path))
                            DefaultFilesOptions.FileProvider = StaticFileOptions.FileProvider;
                        if (!string.IsNullOrEmpty(opt.RequestPath))
                            DefaultFilesOptions.RequestPath = StaticFileOptions.RequestPath;
                        DefaultFilesOptions.DefaultFileNames.Clear();
                        foreach (var f in opt.DefaultFiles)
                            DefaultFilesOptions.DefaultFileNames.Add(f);
                    }

                    res.Add((StaticFileOptions, DirectoryBrowserOptions, DefaultFilesOptions));
                }
                return res;
            }
        }

        public override IEnumerable<KeyValuePair<int, Action<IServiceCollection>>> ConfigureServicesActionsByPriorities
        {
            get
            {
                var d = new Dictionary<int, Action<IServiceCollection>>();
                if (Settings.Any(_ => _.DirectoryBrowserOptions != null))
                    d[Priority] = service => service.AddDirectoryBrowser();
                return d;
            }
        }

        public override IEnumerable<KeyValuePair<int, Action<IApplicationBuilder>>> ConfigureActionsByPriorities
        {
            get
            {
                var priority = Priority;
                var d = new Dictionary<int, Action<IApplicationBuilder>>();
                foreach (var setting in Settings)
                {
                    if (setting.DefaultFilesOptions != null)
                        d[priority++] = app => app.UseDefaultFiles(setting.DefaultFilesOptions);

                    if (setting.DirectoryBrowserOptions != null)
                        d[priority++] = app => app.UseDirectoryBrowser(setting.DirectoryBrowserOptions);

                    d[priority++] = app => app.UseStaticFiles(setting.StaticFileOptions);
                }
                return d;
            }
        }

        public class Options
        {
            public string Path { get; set; }
            public string RequestPath { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public Dictionary<string, string> MIMEtypes { get; set; }
            public String[] DefaultFiles { get; set; }
            public bool EnableDirectoryBrowser { get; set; } = false;
        }

        private string ContentPath => Environment?.ContentRootPath ?? Directory.GetCurrentDirectory();
    }
}
