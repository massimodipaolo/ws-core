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
        public Message(){}

        public MessageAddress Sender { get; set; }
        public IEnumerable<MessageAddress> Recipients { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public class MessageAddress
        {
            public string Name { get; set; }
            public string Address { get; set; }
        }
    }

}
