using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ws.Core.Extensions.Message;

public interface IMessage: IHealthCheck
{
    Task SendAsync(Message message, bool throwException = false);
    Task<IEnumerable<Message>> ReceiveAsync();
}
public class Message
{
    public Message() { }

    public Actor Sender { get; set; } = new();
    public IEnumerable<Actor> Recipients { get; set; } = Array.Empty<Actor>();
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessagePriority Priority { get; set; } = MessagePriority.Normal;
    /// <summary>
    /// Set Implementation-specific message format
    /// </summary>
    public string Format { get; set; } = string.Empty;
    public IEnumerable<Attachment> Attachments { get; set; } = Array.Empty<Attachment>();

    /// <summary>
    /// Implementation-specific properties
    /// </summary>
    public dynamic? Arguments { get; set; }
    public class Actor
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ActorType Type { get; set; } = ActorType.Primary;

    }
    public enum ActorType
    {
        Primary,
        Subscriber,
        Logging
    }

    public class Attachment
    {
        public string? Name { get; set; }
        public byte[] Content { get; set; } = Array.Empty<byte>();  
    }
}

public enum MessagePriority
{
    Low = 0,     
    Normal = 1,        
    High = 2
}

