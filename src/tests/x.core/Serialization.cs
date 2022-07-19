using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Serialization : BaseTest
{
    public Serialization(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("minimal", "foo","bar")]
    [InlineData("controller", "foo", "bar")]
    public async Task Serialize_NestedExceptions_ContainsNulls_Success(string apiType, string name, string value)
    {
        // Arrange
        var url = $"/api/{apiType}/exception/argumentOutOfRange/{name}/{value}";

        // Act
        var (response,content) =  await Get_EndpointsResponse(url);

        // Assert: status 200-299
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains(name, content);
        Assert.Contains(value, content);
        Assert.Contains("I AM ERROR", content);
        Assert.Contains("Some out of range error occured", content);
        Assert.Contains("null", content);
    }

}