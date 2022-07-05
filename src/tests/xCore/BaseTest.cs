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

        public async Task Get_EndpointsReturnSuccess(
            string url,
            WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {
            // Act
            var (response, content) = await Get_EndpointsResponse(url, factoryType, clientOptions);

            // Assert: status 200-299
            Assert.True(response.IsSuccessStatusCode);
        }

        public async Task<(HttpResponseMessage response, string content)> Get_EndpointsResponse(
            string url,
            WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(factoryType, clientOptions);

            // Act
            var response = await client.GetAsync(url);
            var content = await _content(response);

            return (response,content);
        }

        public async Task<(HttpResponseMessage response, string content)> Post_EndpointsResponse(
            string url,
            HttpContent? value,
            WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(factoryType, clientOptions);

            // Act
            var response = await client.PostAsync(url,value);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> Put_EndpointsResponse(
            string url,
            HttpContent? value,
            WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(factoryType, clientOptions);

            // Act
            var response = await client.PutAsync(url, value);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> Delete_EndpointsResponse(
            string url,
            WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(factoryType, clientOptions);

            // Act
            var response = await client.DeleteAsync(url);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> DeleteMany_EndpointsResponse(
            string url,
            HttpContent? value,
            WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(factoryType, clientOptions);

            // Act
            var response = await client.SendAsync(new HttpRequestMessage() { RequestUri = new Uri($"{client.BaseAddress.OriginalString}{url}"), Method = HttpMethod.Delete, Content = value});
            var content = await _content(response);

            return (response, content);
        }

        protected HttpClient _client(WebApplicationFactoryType factoryType = WebApplicationFactoryType.Development,WebApplicationFactoryClientOptions? clientOptions = null)
        {
            var factory = GetFactory(factoryType);
            return factory.CreateClient(clientOptions ?? new WebApplicationFactoryClientOptions() { });
        }

        protected async Task<string> _content(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content?.Headers?.ContentType?.ToString() ?? "";
            if (contentType.Contains("json"))
            {
                var jdoc = JsonDocument.Parse(content);
                content = JsonSerializer.Serialize(jdoc, new JsonSerializerOptions { WriteIndented = true });
            }
            _output.Write(
                $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri?.ToString() ?? ""}", 
                response.StatusCode.ToString(), 
                response.Headers.ToString(),
                response.Content?.Headers?.ToString() ?? "",
                content);
            return content;
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
