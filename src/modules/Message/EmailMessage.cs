using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Extensions.Message
{
    public class EmailMessage : IMessage
    {
        private ILogger<IMessage> _logger { get; set; }
        private IMessageConfiguration _config { get; set; }
        public EmailMessage(ILogger<IMessage> logger, IMessageConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task SendAsync(Message message)
        {
            var sender = _config.Senders.FirstOrDefault();
            if (sender != null && string.IsNullOrEmpty(sender.Address))
            {
                var mime = new MimeMessage();
                mime.From.Add(new MailboxAddress(message.Sender.Name, message.Sender.Address));
                mime.To.AddRange(message.Recipients.Select(x => new MailboxAddress(x.Name, x.Address)));
                //mime.Cc
                //mime.Bcc

                mime.Subject = message.Subject;
                mime.Body = new TextPart(TextFormat.Html)
                {
                    Text = message.Content
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(sender.Address, sender.Port == 0 ? 25 : sender.Port);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (!string.IsNullOrEmpty(sender.UserName) && !string.IsNullOrEmpty(sender.Password))
                        client.Authenticate(sender.UserName, sender.Password);
                    await client.SendAsync(mime)
                        .ContinueWith(async t => await client.DisconnectAsync(true));
                    //await client.DisconnectAsync(true);
                }
            }

        }
        public async Task<IEnumerable<Message>> ReceiveAsync()
        {
            var receiver = _config.Receivers.FirstOrDefault();
            if (receiver != null && !string.IsNullOrEmpty(receiver.Address))
            {
                using (var client = new MailKit.Net.Pop3.Pop3Client())
                {
                    client.Connect(receiver.Address, receiver.Port == 0 ? 110 : receiver.Port);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (!string.IsNullOrEmpty(receiver.UserName) && !string.IsNullOrEmpty(receiver.Password))
                        client.Authenticate(receiver.UserName, receiver.Password);
                    List<Message> emails = new List<Message>();
                    for (int i = 0; i < client.Count && i < 10; i++)
                    {
                        var mime = await client.GetMessageAsync(i);
                        var message = new Message
                        {
                            Subject = mime.Subject,
                            Content = !string.IsNullOrEmpty(mime.HtmlBody) ? mime.HtmlBody : mime.TextBody
                        };
                        var _from = (MailboxAddress)mime.From.FirstOrDefault();
                        message.Sender = new Message.MessageAddress { Name = _from.Name, Address = _from.Address };
                        message.Recipients = mime.To.Select(x => (MailboxAddress)x).Select(x => new Message.MessageAddress { Name = x.Name, Address = x.Address });
                        emails.Add(message);
                    }
                    return emails;
                }
            }
            return null;
        }
    }
}
