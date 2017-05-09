using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace core.Extension
{
    public class StaticFiles : ExtensionBase
    {
        //TODO: Inject IFileProvider (or ServiceLocator): https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers
        private IFileProvider _fileProvider => serviceProvider.GetService<IFileProvider>();

        //TODO: singleton
        private (StaticFileOptions StaticFileOptions, DirectoryBrowserOptions DirectoryBrowserOptions, DefaultFilesOptions DefaultFilesOptions) _options
        {
            get
            {                
                Options _opt = GetOptions<Options>();
                
                //StaticFileOptions
                var StaticFileOptions = new StaticFileOptions();
                if (_opt.Headers != null)
                {
                    StaticFileOptions.OnPrepareResponse = ctx =>
                    {
                        foreach (var h in _opt.Headers)
                            ctx.Context.Response.Headers.Append(h.Key, h.Value);
                    };
                }
                if (_opt.MIMEtypes != null)
                {
                    var provider = new FileExtensionContentTypeProvider();
                    foreach (var t in _opt.MIMEtypes)
                        provider.Mappings[t.Key] = t.Value;
                    StaticFileOptions.ContentTypeProvider = provider;
                }

                //DirectoryBrowser
                DirectoryBrowserOptions DirectoryBrowserOptions = null;
                if (_opt.EnableDirectoryBrowser==true)                
                    DirectoryBrowserOptions = new DirectoryBrowserOptions();                    

                //DefaultFiles
                DefaultFilesOptions DefaultFilesOptions = null;
                if (_opt.DefaultFiles?.Length > 0)
                {
                    DefaultFilesOptions = new DefaultFilesOptions();
                    DefaultFilesOptions.DefaultFileNames.Clear();
                    foreach (var f in _opt.DefaultFiles)
                        DefaultFilesOptions.DefaultFileNames.Add(f);
                }

                return (StaticFileOptions, DirectoryBrowserOptions, DefaultFilesOptions);
            }
        }

        public override IEnumerable<KeyValuePair<int, Action<IServiceCollection>>> ConfigureServicesActionsByPriorities
        {
            get
            {
                var priority = Priority;
                var d = new Dictionary<int, Action<IServiceCollection>>();
                if (_options.DirectoryBrowserOptions != null)
                    d[priority] = service => service.AddDirectoryBrowser();                    
                return d;
            }
        }

        public override IEnumerable<KeyValuePair<int, Action<IApplicationBuilder>>> ConfigureActionsByPriorities
        {
            get
            {
                var priority = Priority;
                var d = new Dictionary<int, Action<IApplicationBuilder>>();

                if (_options.DefaultFilesOptions != null)
                    d[priority] = app => app.UseDefaultFiles(_options.DefaultFilesOptions);

                if (_options.DirectoryBrowserOptions != null)
                    d[priority+1] = app => app.UseDirectoryBrowser(_options.DirectoryBrowserOptions);

                d[priority+2] = app => app.UseStaticFiles(_options.StaticFileOptions);

                return d;
            }
        }

        //TODO: IEnumerable<Options>, Path, RequestPath: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files#enabling-directory-browsing
        public class Options
        {   
            public Dictionary<string, string> Headers { get; set; }         
            public Dictionary<string, string> MIMEtypes { get; set; }            
            public String[] DefaultFiles { get; set; }            
            public bool EnableDirectoryBrowser { get; set; } = false;                                
        }

    }
}
