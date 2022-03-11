using System;
using System.Threading.Tasks;
using Ws.Core.Extensions.Message;
using xCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace xCore
{
    public class Message: BaseTest
    {
        private IMessage _service { get; set; }
        public Message(Program<Startup> factory, ITestOutputHelper output) : base(factory, output) { 
            _service = (IMessage)factory.Services.GetService(typeof(IMessage));
        }

        [Theory]
        [InlineData("massimodipaolo@users.noreply.github.com", "massimodipaolo@users.noreply.github.com")]
        public async Task Send_Message(string sender, string recipient) {
            // Arrange
            Exception _ex = null;
            var message = new Ws.Core.Extensions.Message.Message()
            {
                Sender = new Ws.Core.Extensions.Message.Message.Actor() { Address = sender, Name = sender },
                Recipients = new Ws.Core.Extensions.Message.Message.Actor[]
                {
                    new() { Address = recipient, Name = recipient, Type = Ws.Core.Extensions.Message.Message.ActorType.Primary }
                },
                Subject = $"Subject {sender.Split('@')[1]} {DateTime.UtcNow.ToShortTimeString()}",
                Content = "Message content"
            };

            // Act
            try
            {
                await _service.SendAsync(message,throwException: true);
            } catch(Exception ex)
            {
                _ex = ex;
                _output.Write(_service.GetType().ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(_ex));
            }

            // Assert
            Assert.Equal("", _ex?.Message ?? "");
        }

    }
}
