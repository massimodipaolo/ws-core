using core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Extensions.Spa
{
    public class ResponseCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICache _cache;
        private readonly Options.PrerenderingOptions.CacheResponseOptions _options;

        public ResponseCacheMiddleware(ICache cache, RequestDelegate next, Options.PrerenderingOptions.CacheResponseOptions options)
        {
            _next = next;
            _cache = cache;
            _options = options;
        }

        public async Task Invoke(HttpContext ctx)
        {
            if (IsCachable(ctx))
            {
                var key = $"{typeof(ResponseCacheMiddleware).Name}-{ctx.Request.Path}";

                // check cache
                var cachedResponse = _cache.Get<string>(key);
                if (cachedResponse != null)
                {
                    ctx.Response.Headers.Add("Content-Type", "text/html");
                    await ctx.Response.WriteAsync(cachedResponse);
                }
                else
                {
                    Stream stream = ctx.Response.Body;
                    ctx.Response.Body = new MemoryStream();

                    await _next(ctx);

                    // fill response
                    ctx.Response.Body.Seek(0, SeekOrigin.Begin);
                    string text = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
                    ctx.Response.Body.Seek(0, SeekOrigin.Begin);

                    // set cache
                    if (ctx.Response.StatusCode == 200)
                        _cache.Set(key, text, CacheEntryOptions.Expiration.Never);

                    await ctx.Response.Body.CopyToAsync(stream);

                }
            }
            else
            {
                await _next(ctx);
            }
        }

        private bool IsCachable(HttpContext ctx)
        {
            var rq = ctx.Request;

            return
                (
                    (!_options.SkipQueryStringPath || string.IsNullOrWhiteSpace(rq.QueryString.Value)) // don't cache parameterized path
                    && (!_options.SkipFilePath || string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(rq.Path))) // only extensionless resource
                    && (_options.ExcludePaths == null || !_options.ExcludePaths.Any(_ => rq.Path.StartsWithSegments(_))) // exclude specific prefixes
                )
                ||
                (
                    _options.IncludePaths != null && _options.IncludePaths.Any(_ => rq.Path.StartsWithSegments(_)) // always include these prefixes
                );
        }
    }
}
