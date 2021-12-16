using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ws.Core;
using Ws.Core.Extensions.Data.Cache;
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
        [InlineData("/swagger/public/swagger.json")]
        public async Task Get_Endpoints(string url) => await Get_EndpointsReturnSuccess(url);        

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