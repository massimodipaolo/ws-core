using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace xCore
{
    public class Api : BaseTest
    {
        public Api(Program factory, ITestOutputHelper output) : base(factory, output) {}

        [Theory]
        [InlineData("/swagger")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            _output.Write(url, response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(
                "text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString()
                );
        }

        /*
        [Fact]
        public async Task Get_Diagnostic(Ws.Core.Extensions.Data.Cache.ICache cache, IConfiguration config, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, Microsoft.Extensions.Hosting.IHostApplicationLifetime applicationLifetime,Microsoft.AspNetCore.Http.IHttpContextAccessor ctx)
        {
            // Arrange
            var controller = new Controllers.Diagnostic(cache, config, env, applicationLifetime, ctx);

            // Act
            var response = controller.Get();
            var exec = await response.ExecuteResultAsync(null).ConfigureAwait(false);
            _output.Write(await response.ExecuteResultAsync());

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(
                "text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString()
                );
        }
        */
    }
}