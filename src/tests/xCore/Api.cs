using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace xCore;

public class Api : BaseTest
{
    public Api(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("/swagger")]
    [InlineData("/swagger/public/swagger.json")]
    [InlineData($"/api/diagnostic?culture=en-US")]
    [InlineData("/foo?culture=it-IT")]
    [InlineData("/api/log")]
    [InlineData("/api/log/100")]
    [InlineData($"/api/app/{nameof(xCore.Models.Agenda)}")]
    public async Task Get_MockEndpoints(string url) => await Get_EndpointsReturnSuccess(url, WebApplicationFactoryType.Mock);

    [Theory]
    [InlineData("/")]
    [InlineData("/api/log")]
    [InlineData("/api/log/100")]
    [InlineData($"/api/app/{nameof(xCore.Models.Agenda)}")]
    [InlineData($"/api/app/{nameof(xCore.Models.User)}/1")]
    [InlineData($"/api/app/{nameof(xCore.Models.Todo)}/1")]
    [InlineData($"/api/store/{nameof(xCore.Models.Store.Product)}/1")]
    public async Task Get_DevelopmentEndpoints(string url) => await Get_EndpointsReturnSuccess(url, WebApplicationFactoryType.Development);

}