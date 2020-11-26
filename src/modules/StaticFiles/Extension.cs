using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Ws.Core.Extensions.Base;
using static Ws.Core.Extensions.StaticFiles.Options;
using Microsoft.AspNetCore.Hosting;

namespace Ws.Core.Extensions.StaticFiles
{
    public class Extension : Base.Extension
    {
        private IEnumerable<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)> Settings
        {
            get
            {
                Options opts = GetOptions<Options>();
                var res = new List<(StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions)>();

                if (opts == null || opts.Paths == null || !opts.Paths.Any())
                {
                    opts = new Options()
                    {
                        Paths = new Options.FolderOption[] {
                        new Options.FolderOption() {}
                    }
                    };
                }

                foreach (var opt in opts.Paths)
                {
                    if (opt != null)
                    {
                        //StaticFileOptions
                        var staticFileOptions = opt.GetStaticFileOptions(env?.ContentRootPath ?? Directory.GetCurrentDirectory(),logger);

                        //DirectoryBrowser
                        DirectoryBrowserOptions directoryBrowserOptions = null;
                        if (opt.EnableDirectoryBrowser)
                        {
                            directoryBrowserOptions = new DirectoryBrowserOptions();
                            if (!string.IsNullOrEmpty(opt.Path))
                                directoryBrowserOptions.FileProvider = staticFileOptions.FileProvider;
                            if (!string.IsNullOrEmpty(opt.RequestPath))
                                directoryBrowserOptions.RequestPath = staticFileOptions.RequestPath;
                        }

                        //DefaultFiles
                        DefaultFilesOptions defaultFilesOptions = null;
                        if (opt.DefaultFiles?.Length > 0)
                        {
                            defaultFilesOptions = new DefaultFilesOptions();
                            if (!string.IsNullOrEmpty(opt.Path))
                                defaultFilesOptions.FileProvider = staticFileOptions.FileProvider;
                            if (!string.IsNullOrEmpty(opt.RequestPath))
                                defaultFilesOptions.RequestPath = staticFileOptions.RequestPath;
                            defaultFilesOptions.DefaultFileNames.Clear();
                            foreach (var f in opt.DefaultFiles)
                                defaultFilesOptions.DefaultFileNames.Add(f);
                        }

                        res.Add((staticFileOptions, directoryBrowserOptions, defaultFilesOptions));
                    }
                }

                return res;
            }
        }

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            if (Settings.Any(_ => _.DirectoryBrowserOptions != null))
                serviceCollection.AddDirectoryBrowser();
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            foreach (var setting in Settings)
            {
                if (setting.DefaultFilesOptions != null)
                    applicationBuilder.UseDefaultFiles(setting.DefaultFilesOptions);

                if (setting.DirectoryBrowserOptions != null)
                    applicationBuilder.UseDirectoryBrowser(setting.DirectoryBrowserOptions);

                applicationBuilder.UseStaticFiles(setting.StaticFileOptions);
            }
        }
    }
}
