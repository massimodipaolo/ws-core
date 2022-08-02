using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Diagnostic : BaseTest
{
    Diagnostic(Program factory) : base(factory) { }
    public Diagnostic(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData($"/{nameof(Extensions)}/Ws.Core.Extensions.Diagnostic?culture=en-US")]
    public async Task Get_DevelopmentEndpoints(string url) => await Get_EndpointsReturnSuccess(url);

}