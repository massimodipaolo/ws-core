using Microsoft.AspNetCore.Mvc.Testing;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.HealthCheck.Checks.AppLog;
using Xunit;
using Xunit.Abstractions;
using AppLog = Ws.Core.Extensions.HealthCheck.Checks.AppLog;

namespace xCore
{
    public class ImageProcessor : BaseTest
    {
        public ImageProcessor(Program factory, ITestOutputHelper output) : base(factory, output) { }

        [Theory]
        [InlineData("/media/sixlabors.imagesharp.web.png", "image/png", "18945", "43200", false)] // source static file
        //[InlineData("/media/sixlabors.imagesharp.web.png?width=300&format=gif", "image/gif", "29704", "604800", true)] // processed new rq
        [InlineData("/media/sixlabors.imagesharp.web.png?width=300&format=gif", "image/gif", "29704", "604800", false)] // processed in-cache
        public async Task Get_Image(string url, string contentType, string contentLength, string cacheMaxAge, bool emptyCache)
        {
            if (emptyCache)
                System.IO.Directory.Delete($"{AppDomain.CurrentDomain.BaseDirectory}wwwroot\\is-cache",true);

            var (response, _) = await Get_EndpointsResponse(url);
            response.EnsureSuccessStatusCode();

            _ = response.Content.Headers.TryGetValues("Content-Type", out var _contentType);
            Assert.True(_contentType?.First() == contentType);

            _ = response.Content.Headers.TryGetValues("Content-Length", out var _contentLength);
            Assert.True(_contentLength?.First() == contentLength);

            _ = response.Headers.TryGetValues("Cache-Control", out var _cacheControl);
            Assert.True(_cacheControl?.First().Contains($"max-age={cacheMaxAge}"));
        }
    }
}
