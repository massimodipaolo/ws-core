using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace core.Extensions.Message
{
    public class EmailMessage : IMessage
    {
        private ILogger<IMessage> _logger { get; set; }
        //TODO: implement https://github.com/jstedfast/MailKit
        public EmailMessage(ILogger<IMessage> logger)
        {
            _logger = logger;
        }
        public Task SendAsync(string sender, string[][] recipients, string subject, string body)
        {
            throw new NotImplementedException();
        }
        public Task ReceiveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
