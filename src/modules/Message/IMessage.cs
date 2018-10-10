using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace core.Extensions.Message
{
    public interface IMessage
    {
        Task SendAsync(Message message);
        Task<IEnumerable<Message>> ReceiveAsync();
    }
    public class Message
    {
        public Message() { }

        public Actor Sender { get; set; }
        public IEnumerable<Actor> Recipients { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }

        /// <summary>
        /// Implementation-specific model
        /// </summary>
        public dynamic Model { get; set; }
        public class Actor
        {
            public string Name { get; set; }
            public string Address { get; set; }
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
            public string Name { get; set; }
            public byte[] Content { get; set; }
        }
    }

}
