using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Ws.Core.Shared.StaticFilesFolder
{
    public class Options
    {
        [Description("Relative or UNC path")]
        public string Path { get; set; }
        [Description("Virtual path, i.e. /downloads")]
        public string RequestPath { get; set; }
        [Description("Dictionary of key/value response header, i.e. \"Cache-Control\": \"public,max-age=43200\"")]
        public Dictionary<string, string> Headers { get; set; }
        [Description("Dictionary of key/value extention/content-type, i.e. \".myapp\": \"application/x-msdownload\"")]
        public Dictionary<string, string> MIMEtypes { get; set; }
        private bool? _isRelativePath;
        public bool IsRelativePath
        {
            get
            {
                if (_isRelativePath == null)
                    _isRelativePath = Path != null && System.Text.RegularExpressions.Regex.IsMatch(Path, @"^([a-z]:)*(\/*(\.*[a-z0-9]+\/)*(\.*[a-z0-9]+))");

                return _isRelativePath.Value;
            }
            set { _isRelativePath = value; }
        }
        public string Root(string basePath) =>
            IsRelativePath
            ?
            System.IO.Path.Combine(basePath, Path)
            :
            Path;
        public IFileProvider FileProvider(string basePath) => new PhysicalFileProvider(Root(basePath));

        public StaticFileOptions GetStaticFileOptions(string basePath, ILogger logger)
        {
            var staticFileOptions = new StaticFileOptions();
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    staticFileOptions.FileProvider = FileProvider(basePath);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "");
                }
            }
            if (!string.IsNullOrEmpty(RequestPath))
                staticFileOptions.RequestPath = new PathString(RequestPath);
            if (Headers != null)
            {
                staticFileOptions.OnPrepareResponse = ctx =>
                {
                    foreach (var h in Headers)
                        ctx.Context.Response.Headers.Append(h.Key, h.Value);
                };
            }
            if (MIMEtypes != null)
            {
                var provider = new FileExtensionContentTypeProvider();
                foreach (var t in MIMEtypes)
                    provider.Mappings[t.Key] = t.Value;
                staticFileOptions.ContentTypeProvider = provider;
            }
            return staticFileOptions;
        }

    }
}
