using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Gateway : BaseTest
{
    public Gateway(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("/bing/foo")] // route mapped
    [InlineData("/")] // not-mapped
    public async Task Get_Endpoints(string url) => await Get_EndpointsReturnSuccess(
        url, 
        clientOptions: new WebApplicationFactoryClientOptions() { HandleCookies = false, AllowAutoRedirect = false});
}
