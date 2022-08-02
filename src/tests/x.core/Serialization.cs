using x.core.Extensions;
using x.core.Models;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Serialization : BaseTest
{
    Serialization(Program factory) : base(factory) { }
    public Serialization(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("minimal", "foo", "bar")]
    [InlineData("controller", "foo", "bar")]
    public async Task Serialize_NestedExceptions_ContainsNulls_Success(string apiType, string name, string value)
    {
        // Arrange
        var url = $"/api/{apiType}/exception/argumentOutOfRange/{name}/{value}";

        // Act
        var (response, content) = await Get_EndpointsResponse(url);

        // Assert: status 200-299
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains(name, content);
        Assert.Contains(value, content);
        Assert.Contains("I AM ERROR", content);
        Assert.Contains("Some out of range error occured", content);
        Assert.Contains("null", content);
    }

    [Fact]
    public void Mask_SensitiveData()
    {
        MaskedUser user = new() { 
            Id = 1, Name = "Joe Doe",
            Email = "secret@email.com",
            Phone = "123456789",
            Address = new() { Street = "No name street", City = "No name city", Geo = new() {Lat="10", Lng = "10"} },  
            Company = new() { Name = "Nasa", CatchPhrase = "#htfu" }, 
            Enemies = new[] { new Company() { Name = "U.R.S.S.", CatchPhrase = "!!!" } },
            Posts = new[] { new Post() { Id = 1, Title ="Public title", Body="Not so secret text", UserId = 1, Comments = new[] { new Comment() { Id=1, PostId = 1, Name="Comment name", Body="secret comment", Email="author@email.com"} } } },
            CreatedAt = DateTime.Now };

        var _serialized = System.Text.Json.JsonSerializer.Serialize((MaskedUser)Ws.Core.Shared.Serialization.Obfuscator.MaskSensitiveData(user));
        var maskeredValues = _serialized.Split("***").Length - 1;
        _output?.Write(_serialized, maskeredValues.ToString());

        Assert.Equal(8, maskeredValues);


    }

}