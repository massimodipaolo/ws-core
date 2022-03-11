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
        public Api(Program<Startup> factory, ITestOutputHelper output) : base(factory, output) {}

        [Theory]
        [InlineData("/swagger")]
        [InlineData("/swagger/public/swagger.json")]
        [InlineData("/api/diagnostic")]
        public async Task Get_Endpoints(string url) => await Get_EndpointsReturnSuccess(url);        
    }
}