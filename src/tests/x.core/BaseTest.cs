using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using x.core.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace x.core
{
    public class BaseTest : IClassFixture<Program>
    {
        protected Program _factory;
        protected readonly ITestOutputHelper? _output;
        protected readonly JsonSerializerOptions? _jsonSerializerOptions;

        public BaseTest(Program factory)
        {
            _factory = factory;
            if (_factory.Services.GetService(typeof(Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>)) is Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions> jsonOptions)
                _jsonSerializerOptions = jsonOptions.Value.SerializerOptions;
        }

        public BaseTest(Program factory, ITestOutputHelper output) : this(factory)
        {
            _output = output;
        }

        public async Task Get_EndpointsReturnSuccess(
            string url,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {
            // Act
            var (response, content) = await Get_EndpointsResponse(url, clientOptions);

            // Assert: status 200-299
            Assert.True(response.IsSuccessStatusCode);
        }

        public async Task<(HttpResponseMessage response, string content)> Get_EndpointsResponse(
            string url,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(clientOptions);

            // Act
            var response = await client.GetAsync(url);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> Post_EndpointsResponse(
            string url,
            HttpContent? value,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(clientOptions);

            // Act
            var response = await client.PostAsync(url, value);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> Put_EndpointsResponse(
            string url,
            HttpContent? value,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(clientOptions);

            // Act
            var response = await client.PutAsync(url, value);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> Delete_EndpointsResponse(
            string url,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(clientOptions);

            // Act
            var response = await client.DeleteAsync(url);
            var content = await _content(response);

            return (response, content);
        }

        public async Task<(HttpResponseMessage response, string content)> DeleteMany_EndpointsResponse(
            string url,
            HttpContent? value,
            WebApplicationFactoryClientOptions? clientOptions = null)
        {

            // Arrange
            var client = _client(clientOptions);

            // Act
            var response = await client.SendAsync(new HttpRequestMessage() { RequestUri = new Uri($"{client.BaseAddress?.OriginalString}{url}"), Method = HttpMethod.Delete, Content = value });
            var content = await _content(response);

            return (response, content);
        }

        protected HttpClient _client(WebApplicationFactoryClientOptions? clientOptions = null)
        => _factory.CreateClient(clientOptions ?? new WebApplicationFactoryClientOptions() { });

        protected object? _getService(Type type)
        {
            Func<IServiceProvider, object?> _get = (provider) => provider.GetService(type);
            object? _service = null;
            try
            {
                _service = _get(_factory.Services);
            }
            catch(Exception ex)
            {
                _output?.Write($"Error getting service {type}\n: {ex.Message}");
                using var scope = _factory.Services.CreateScope();
                _service = _get(scope.ServiceProvider);
            }
            return _service;
        }

        protected async Task<string> _content(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content?.Headers?.ContentType?.ToString() ?? "";
            if (contentType.Contains("json"))
            {
                var jdoc = JsonDocument.Parse(content);
                content = JsonSerializer.Serialize(jdoc, _jsonSerializerOptions);
            }
            _output?.Write(
                $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri?.ToString() ?? ""}",
                response.StatusCode.ToString(),
                response.Headers.ToString(),
                response.Content?.Headers?.ToString() ?? "",
                content);
            return content;
        }

        public void Check_ServiceImplementation(Type Tinterface, Type ExpectedTimplementation)
        {
            // Arrange
            object? _service = _getService(Tinterface);          

            // Act
            _output?.Write($"Interface: {Tinterface}\n Expected: {ExpectedTimplementation}\n Implementation: {_service}");

            // Assert
            Assert.Equal(ExpectedTimplementation, _service?.GetType());
        }
    }
}

namespace x.core.Extensions
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
