using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Diagnostic : BaseTest
{
    public Diagnostic(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData($"/{nameof(Extensions)}/Ws.Core.Extensions.Diagnostic?culture=en-US")]
    public async Task Get_DevelopmentEndpoints(string url) => await Get_EndpointsReturnSuccess(url, WebApplicationFactoryType.Development);

}