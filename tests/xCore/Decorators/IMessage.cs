using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Ws.Core.Extensions.Message;

namespace xCore.Decorators
{
    public class IMessageRetry: IMessage
    {
        private readonly Ws.Core.Extensions.Message.IMessage _inner;
        private readonly Polly.IAsyncPolicy _retryPolicy;

        public IMessageRetry(Ws.Core.Extensions.Message.IMessage inner)
        {
            _inner = inner;
            _retryPolicy = Polly.Policy
                .Handle<ArgumentOutOfRangeException>()
                .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public Task SendAsync(Ws.Core.Extensions.Message.Message message, bool throwException = false)
        {
            return _retryPolicy.ExecuteAsync(
                async () => await _inner.SendAsync(
                    message, 
                    throwException: true // force true
                    ));
        }

        public Task<IEnumerable<Ws.Core.Extensions.Message.Message>> ReceiveAsync()
        => _inner.ReceiveAsync();
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => _inner.CheckHealthAsync(context, cancellationToken);
    }

    public class IMessageLogger: IMessage
    {
        private readonly Ws.Core.Extensions.Message.IMessage _inner;
        private readonly ILogger<Ws.Core.Extensions.Message.IMessage> _logger;

        public IMessageLogger(Ws.Core.Extensions.Message.IMessage inner, ILogger<Ws.Core.Extensions.Message.IMessage> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task SendAsync(Ws.Core.Extensions.Message.Message message, bool throwException = false)
        {
            _logger.LogInformation($"Sending message: {message.Subject} to {message.Recipients.FirstOrDefault(_ => _.Type ==  Ws.Core.Extensions.Message.Message.ActorType.Primary)}");
            await _inner.SendAsync(message,throwException);
        }

        public Task<IEnumerable<Ws.Core.Extensions.Message.Message>> ReceiveAsync()
        => _inner.ReceiveAsync();
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => _inner.CheckHealthAsync(context, cancellationToken);
    }

    public class IMessageCopy : IMessage
    {
        private readonly Ws.Core.Extensions.Message.IMessage _inner;
        private readonly ILogger<Ws.Core.Extensions.Message.IMessage> _logger;

        public IMessageCopy(Ws.Core.Extensions.Message.IMessage inner, ILogger<Ws.Core.Extensions.Message.IMessage> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task SendAsync(Ws.Core.Extensions.Message.Message message, bool throwException = false)
        {
            var cc = new Ws.Core.Extensions.Message.Message.Actor
            {
                Type = Ws.Core.Extensions.Message.Message.ActorType.Logging,
                Name = "ws-core cc",
                Address = "ws-core.cc@users.noreply.github.com"
            };
            message.Recipients = message.Recipients?.Append(cc);
            
            _logger.LogInformation($"Adding message cc: {cc.Address}");
            await _inner.SendAsync(message, throwException);
        }

        public Task<IEnumerable<Ws.Core.Extensions.Message.Message>> ReceiveAsync()
        => _inner.ReceiveAsync();
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => _inner.CheckHealthAsync(context, cancellationToken);
    }

    public class IMessageSignature : IMessage
    {
        private readonly Ws.Core.Extensions.Message.IMessage _inner;

        public IMessageSignature(Ws.Core.Extensions.Message.IMessage inner)
        {
            _inner = inner;
        }

        public async Task SendAsync(Ws.Core.Extensions.Message.Message message, bool throwException = false)
        {
            var newLine = message.Format?.Contains("html") == true ? "<br/>" : Environment.NewLine;
            message.Content += $"{newLine}{newLine}{newLine}---------------------{newLine}©🆆🆂-🅲🅾🆁🅴";
            await _inner.SendAsync(message, throwException);
        }

        public Task<IEnumerable<Ws.Core.Extensions.Message.Message>> ReceiveAsync()
        => _inner.ReceiveAsync();
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => _inner.CheckHealthAsync(context, cancellationToken);
    }
}
