using Microsoft.AspNetCore.Mvc.Testing;
using Ws.Core;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using xCore.Extensions;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace xCore

{
    public class BaseTest : IClassFixture<Program<Startup>>
    {
        protected readonly Program<Startup> _factory;
        protected readonly ITestOutputHelper _output;
        public BaseTest(Program<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        public async Task Get_EndpointsReturnSuccess(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
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

        public void Check_ServiceImplementation(Type Tinterface, Type ExpectedTimplementation)
        {
            // Arrange
            var _service = _factory.Services?.GetService(Tinterface);

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
