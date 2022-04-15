using Xunit;
using Xunit.Abstractions;
using System.Text.Json;
using xCore.Extensions;
using xCore.Endpoints;

namespace xCore
{
    public class Crud : BaseTest
    {
        public Crud(Program factory, ITestOutputHelper output) : base(factory, output) { }

        [Theory]
        [InlineData(WebApplicationFactoryType.Development)]
        [InlineData(WebApplicationFactoryType.Mock)]
        public async Task Check_CrudGetAll(WebApplicationFactoryType factoryType)
        {

            // Arrange
            var factory = GetFactory(factoryType);
            HttpClient client = factory.CreateClient();

            // Act
            var items = await client.GetFromJsonAsync<List<xCore.Endpoints.Todo>>("/api/todo");

            _output.Write(JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true }));

            // Assert
            Assert.NotNull(items);
            Assert.True(factoryType == WebApplicationFactoryType.Development ? items.Any() : !items.Any());
        }

        [Theory]
        [InlineData(WebApplicationFactoryType.Development)]
        [InlineData(WebApplicationFactoryType.Mock)]
        public async Task Check_CrudCreate(WebApplicationFactoryType factoryType)
        {
            // Arrange
            var factory = GetFactory(factoryType);
            HttpClient client = factory.CreateClient();
            Todo item = new() { Title = $"You have to _____-{DateTime.Now}" };

            // Act
            var response = await client.PostAsJsonAsync<Todo>("/api/todo", item);
            var created = await response.Content.ReadFromJsonAsync<Todo>();
            _output.Write(JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true }));

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(created);
        }

    }
}