using Ws.Core.Extensions.Message;
using x.core.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Message : BaseTest
{
    private IMessage? _service { get; set; }
    Message(Program factory) : base(factory) { }
    public Message(Program factory, ITestOutputHelper output) : base(factory, output)
    {
        if (factory.Services.GetService(typeof(IMessage)) is IMessage __service)
            _service = __service;
    }

    [Theory]
    [InlineData("ws-core@mail.local.io", "massimo.dipaolo@mail.local.io")]
    public async Task Send_Message(string sender, string recipient)
    {
        Assert.NotNull(_service);

        // Arrange
        Exception? _ex = null;
        var message = new Ws.Core.Extensions.Message.Message()
        {
            Sender = new Ws.Core.Extensions.Message.Message.Actor() { Address = sender, Name = sender },
            Recipients = new Ws.Core.Extensions.Message.Message.Actor[]
            {
                new() { Address = recipient, Name = recipient, Type = Ws.Core.Extensions.Message.Message.ActorType.Primary }
            },
            Subject = $"Subject {sender.Split('@')[1]} {DateTime.UtcNow.ToShortTimeString()}",
            Content = "Message content",
            Arguments = new { arg1 = "foo", arg2 = "bar" },
            Format = "html"
        };
        message.Attachments = new List<Ws.Core.Extensions.Message.Message.Attachment>() {
            new() { Name = "message.txt", Content = System.Text.Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(message, _jsonSerializerOptions))
            } };

        // Act
        if (_service != null)
            try
            {
                await _service.SendAsync(message, throwException: true);
            }
            catch (Exception ex)
            {
                _ex = ex;
                _output?.Write(_service.GetType().ToString(), System.Text.Json.JsonSerializer.Serialize(_ex, _jsonSerializerOptions));
            }

        // Assert
        Assert.Equal("", _ex?.Message ?? "");
    }

    [Fact]
    public async Task Receive_Message()
    {
        Assert.NotNull(_service);
        if (_service != null)
            try
            {
                IEnumerable<Ws.Core.Extensions.Message.Message> messages = await _service.ReceiveAsync();
                _output?.Write(_service.GetType().ToString(), System.Text.Json.JsonSerializer.Serialize(messages?.Take(1)));
                Assert.True(messages is not null && messages.Any());
            }
            catch (Exception ex)
            {
                _output?.Write(_service.GetType().ToString(), System.Text.Json.JsonSerializer.Serialize(ex, _jsonSerializerOptions));
                Assert.True(false);
            }
    }
}
