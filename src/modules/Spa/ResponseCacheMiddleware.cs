using Ws.Core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.Base;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;
using AngleSharp;

namespace Ws.Core.Extensions.Spa
{
    public class ResponseCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICache _cache;
        private readonly IDiscriminator<ResponseCacheMiddleware> _discriminator;
        private readonly Options.CacheResponseOptions _options;

        public ResponseCacheMiddleware(ICache cache, IDiscriminator<ResponseCacheMiddleware> discriminator, RequestDelegate next, Options.CacheResponseOptions options)
        {
            _next = next;
            _cache = cache;
            _discriminator = discriminator;
            _options = options;
        }

        public async Task Invoke(HttpContext ctx)
        {
            var _discriminator_value = _discriminator?.Value ?? ""; // init discriminator
            if (IsCachable(ctx))
            {
                // copy headers
                var headers = new List<KeyValuePair<string, StringValues>>(ctx.Response.Headers.Where(_ => _.Key != null));
                ctx.Response.OnStarting(() =>
                {
                    headers.ForEach(_ =>
                    {
                        if (!ctx.Response.Headers.ContainsKey(_.Key))
                            ctx.Response.Headers.Add(_.Key, _.Value);
                    });
                    return Task.FromResult(0);
                });

                var key = $"{typeof(ResponseCacheMiddleware).Name}-{ctx.Request.Path}-{_discriminator_value}";
                // check cache
                var cachedResponse = _cache.Get<Obj>(key);
                if (cachedResponse != null)
                {
                    // preload hints
                    AddEarlyHints(ctx, cachedResponse.EarlyHints);

                    ctx.Response.Headers.Add("Content-Type", "text/html");
                    await ctx.Response.WriteAsync(cachedResponse.Html);
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

                    // set cache / headers
                    if (ctx.Response.StatusCode == 200)
                    {
                        var hints = await PreloadHints(text);
                        AddEarlyHints(ctx, hints);

                        await _cache.SetObjectAsync(key, new Obj() { Html = text, EarlyHints = hints }, CacheEntryOptions.Expiration.Never);
                    }

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

        private async Task<string> PreloadHints(string html)
        {
            if (_options.AddEarlyHints?.Enable == true)
            {
                var config = AngleSharp.Configuration.Default;
                var context = AngleSharp.BrowsingContext.New(config);
                var document = await context.OpenAsync(req => req.Content(html));
                string baseh = document.GetElementsByTagName("base").FirstOrDefault()?.GetAttribute("href") ?? "";
                string _base(string path)
                => new[] { "http://", "https://" }.Any(schema => path?.StartsWith(schema, StringComparison.OrdinalIgnoreCase) == true) ? "" : baseh;
                string _directive(string path, string type)
                => $"<{_base(path)}{path}>; rel=preload; as={type};{(!_options.AddEarlyHints.AllowServerPush ? "nopush;" : "")}";

                // resources
                var preload = new List<string>();
                var items = _options.AddEarlyHints.MaxItemsPerType;
                foreach (var type in _options.AddEarlyHints.Types)
                {
                    var hints = type switch
                    {
                        "style" => document.GetElementsByTagName("link")?.Where(_ => _.GetAttribute("rel") == "stylesheet")?.Take(items)?.Select(_ => _directive(_.GetAttribute("href"), type)),
                        "script" => document.Scripts?/*.Where(_ => _.Type?.Contains("javascript") == true)?*/.Take(items)?.Select(_ => _directive(_.Source, type)),
                        "image" => document.GetElementsByTagName("img")?.Take(items)?.Select(_ => _directive(_.GetAttribute("src"), type)),
                        _ => null
                    };
                    if (hints != null)
                        preload.AddRange(hints);
                }
                return string.Join(",", preload);
            }
            return null;
        }

        private static void AddEarlyHints(HttpContext ctx,string hints)
        {
            if (!string.IsNullOrEmpty(hints))
                ctx.Response.Headers.Append("Link", hints);
        }
        public class Obj
        {
            public string Html { get; set; }
            public string EarlyHints { get; set; }
        }
    }
}
