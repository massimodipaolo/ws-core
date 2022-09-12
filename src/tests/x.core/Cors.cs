using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Cors : BaseTest
{
    Cors(Program factory) : base(factory) { }
    public Cors(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("https://unknown.origin.com", "GET", false)] // Disallow for unlisted origin
    [InlineData("https://foo.bar.com", "GET", true)] // Allow any GET for listed origin
    [InlineData("https://foo.bar.com", "POST", false)] // Restrict POST to allowed origins
    [InlineData("https://localhost:60935", "GET", true)] // Allow verb to origin
    [InlineData("https://localhost:60935", "POST", true)] // Allow verb to origin
    public async Task Get_Endpoints(string origin, string method, bool expectedResult)
    {
        // Arrange
        var client = _client();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/app/User");
        request.Headers.Add("Access-Control-Request-Headers", "authorization,content-type");
        request.Headers.Add("Access-Control-Request-Method", method);
        request.Headers.Add("Origin", origin);
        var response = await client.SendAsync(request);
        var content = await _content(response);

        // Assert
        //response.EnsureSuccessStatusCode();
        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var allowedOrigins);
        response.Headers.TryGetValues("Access-Control-Allow-Methods", out var allowedMethods);
        Assert.True(expectedResult == 
            (
                (allowedOrigins ?? Array.Empty<string>()).Contains(origin) 
                && 
                (allowedMethods ?? Array.Empty<string>()).SelectMany(_ => _.Split(',')).Contains(method))
            );
    }

}