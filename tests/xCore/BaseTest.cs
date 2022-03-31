using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using xCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace xCore
{
    public class BaseTest : IClassFixture<Program>
    {
        protected Program _factory;
        protected readonly ITestOutputHelper _output;
        public BaseTest(Program factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }
        protected WebApplicationFactory<Program> GetFactory(WebApplicationFactoryType factoryType)
        => factoryType switch
            {
                WebApplicationFactoryType.Development => _factory,
                WebApplicationFactoryType.Local => new LocalApplicationFactory(),
                WebApplicationFactoryType.Mock => new MockApplicationFactory(),
                _ => _factory
            };
        
        public async Task Get_EndpointsReturnSuccess(string url, WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development)
        {

            // Arrange
            var factory = GetFactory(factoryType);         
            HttpClient client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (response.Content?.Headers?.ContentType?.ToString()?.Contains("json") == true)
                try
                {
                    var jdoc = JsonDocument.Parse(content);
                    content = JsonSerializer.Serialize(jdoc, new JsonSerializerOptions { WriteIndented = true });
                }
                catch { }

            _output.Write(url, response.StatusCode.ToString(), response.Headers.ToString(), response.Content.Headers.ToString(), content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }


        public void Check_ServiceImplementation(Type Tinterface, Type ExpectedTimplementation, WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development)
        {
            // Arrange
            var factory = GetFactory(factoryType);
            object? _service;
            using (var scope = factory.Services.CreateScope())
            {
                _service = scope?.ServiceProvider?.GetService(Tinterface);
            }

            // Act
            _output.Write($"Interface: {Tinterface}\n Expected: {ExpectedTimplementation}\n Implementation: {_service}");

            // Assert
            Assert.True(_service?.GetType() == ExpectedTimplementation);
        }
    }
}

namespace xCore.Extensions
{
    public static partial class Extend
    {
        const int _separatorSize = 50;
        static readonly Func<char, string> _line = (separator) => $"{Environment.NewLine}{new string(separator, _separatorSize)}{Environment.NewLine}";
        static readonly Func<string> newLine = () => _line('-');
        static readonly Func<string> endLine = () => _line('#');

        public static void Write(this ITestOutputHelper output, params string[] args)
        {
            output.WriteLine($"" +
                $"{Environment.StackTrace.Split("\r\n")[2]}" +
                newLine() +
                string.Join(newLine(), args) +
                $"{endLine()}"
                );
        }
    }
}
