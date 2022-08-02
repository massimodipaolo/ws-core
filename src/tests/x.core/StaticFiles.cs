using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class StaticFiles : BaseTest
{
    StaticFiles(Program factory) : base(factory) { }
    public StaticFiles(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("/media")]
    public async Task Get_DefaultDocument(string url) => await Get_EndpointsReturnSuccess(url);

    [Theory]
    [InlineData("/public")]
    public async Task Get_DirectoryBrowser(string url) => await Get_EndpointsReturnSuccess(url);
}