using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class EF : BaseTest
{
    EF(Program factory) : base(factory) { }
    public EF(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("/api/app/User", "Albums", true, false)] // No navigation props
    [InlineData("/api/app/User/1", "Albums", true, true)] // Custom navigation props by includeNavigationProperties (find)
    [InlineData("/api/app/User/1", "Comments", true, false)] // Custom navigation props by includeNavigationProperties (find)
    [InlineData("/api/app/User/ext/1", "Comments", true, true)] // Custom navigation props by IncludeJoin extension
    [InlineData("/api/store/Product", "Brand", false, true)] // Default navigation props by exclude in IncludeNavigationProperties (list)
    [InlineData("/api/store/Product/1", "Brand", false, true)] // Default navigation props by IncludeNavigationProperties (find)
    public async Task Get_Endpoints(string url, /*Type type,*/ string property, bool isArray, bool hasValue)
    {
        var (response, content) = await Get_EndpointsResponse(url);
        var minimizedContent = Regex.Replace(content, @"\s+", "");
        var searchNotEmptyPattern = isArray ? $"\"{property}\":[{{\"" : $"\"{property}\":{{\"";
        Assert.True(response.IsSuccessStatusCode && hasValue == minimizedContent.Contains(searchNotEmptyPattern));
    }

}