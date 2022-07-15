using x.core.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Injector : BaseTest
{
    public Injector(Program factory, ITestOutputHelper output) : base(factory, output)
    {
    }

    [Theory]
    [InlineData("/?culture=fr-FR", "fr-FR")]
    [InlineData($"/{nameof(Ws.Core.Extensions)}/{nameof(Ws.Core.Extensions.Diagnostic)}?culture=en-US", "en-US")]
    public async Task Get_HeaderCurrentCulture(string url, string expectedCulture)
    {
        // Arrange
        var factory = GetFactory(WebApplicationFactoryType.Development);
        var client = factory.CreateClient();
        // Act
        var response = await client.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();
        response.Headers.TryGetValues("X-culture", out IEnumerable<string> values);
        var culture = values?.FirstOrDefault();
        _output.Write(url, response.StatusCode.ToString(), response.Headers.ToString(), response.Content.Headers.ToString(), content, culture);

        // Assert
        Assert.Equal(expectedCulture, culture);
    }

    [Theory]
    [InlineData("/branch?text=hello-world", "hello-world")]
    public async Task Get_ContentTextByQsOnMap(string url, string expectedContent)
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        _output.Write(url, response.StatusCode.ToString(), response.Headers.ToString(), response.Content.Headers.ToString(), content);

        // Assert
        Assert.Equal(expectedContent, content);
    }

    [Theory]
    [InlineData(typeof(Ws.Core.Extensions.Message.IMessage), typeof(x.core.Decorators.IMessageSignature), WebApplicationFactoryType.Development)]             
    public void Check_DecoratorChainTypeByEnvironment(Type Tinterface, Type ExpectedTimplementation, WebApplicationFactoryType factoryType)
        => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation, factoryType);
}

