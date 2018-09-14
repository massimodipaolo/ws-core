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
        private ILogger<EmailMessage> _logger { get; set; }
        private IMessageConfiguration _config { get; set; }
        public EmailMessage(ILogger<EmailMessage> logger, IMessageConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task SendAsync(Message message)
        {
            var sender = _config.Senders.FirstOrDefault();
            if (sender != null && !string.IsNullOrEmpty(sender.Address))
            {
                var mime = new MimeMessage();
                mime.From.Add(new MailboxAddress(message.Sender.Name, message.Sender.Address));
                mime.To.AddRange(message.Recipients.Where(_ => _.Type == Message.ActorType.Primary).Select(x => new MailboxAddress(x.Name, x.Address)));
                mime.Cc.AddRange(message.Recipients.Where(_ => _.Type == Message.ActorType.Subscriber).Select(x => new MailboxAddress(x.Name, x.Address)));
                mime.Bcc.AddRange(message.Recipients.Where(_ => _.Type == Message.ActorType.Logging).Select(x => new MailboxAddress(x.Name, x.Address)));

                mime.Subject = message.Subject;
                mime.Body = new TextPart(TextFormat.Html)
                {
                    Text = message.Content
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    try
                    {
                        await client.ConnectAsync(sender.Address, sender.Port == 0 ? 25 : sender.Port, MailKit.Security.SecureSocketOptions.Auto).ConfigureAwait(false);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        if (!string.IsNullOrEmpty(sender.UserName) && !string.IsNullOrEmpty(sender.Password))
                            await client.AuthenticateAsync(sender.UserName, sender.Password).ConfigureAwait(false);
                        await client.SendAsync(mime).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,"Send email error");                        
                    }
                    finally
                    {
                        await client.DisconnectAsync(true).ConfigureAwait(false);
                    }
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
                    await client.ConnectAsync(receiver.Address, receiver.Port == 0 ? 110 : receiver.Port).ConfigureAwait(false);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (!string.IsNullOrEmpty(receiver.UserName) && !string.IsNullOrEmpty(receiver.Password))
                        await client.AuthenticateAsync(receiver.UserName, receiver.Password).ConfigureAwait(false);
                    List<Message> emails = new List<Message>();
                    for (int i = 0; i < client.Count && i < 10; i++)
                    {
                        var mime = await client.GetMessageAsync(i).ConfigureAwait(false);
                        var message = new Message
                        {
                            Subject = mime.Subject,
                            Content = !string.IsNullOrEmpty(mime.HtmlBody) ? mime.HtmlBody : mime.TextBody
                        };
                        var _from = (MailboxAddress)mime.From.FirstOrDefault();
                        message.Sender = new Message.Actor { Name = _from.Name, Address = _from.Address };
                        message.Recipients = new List<Message.Actor>();
                        ((List<Message.Actor>)message.Recipients).AddRange(
                            mime.To.Select(_ => (MailboxAddress)_).Select(_ => new Message.Actor { Name = _.Name, Address = _.Address, Type = Message.ActorType.Primary })
                            .Union(mime.Cc.Select(_ => (MailboxAddress)_).Select(_ => new Message.Actor { Name = _.Name, Address = _.Address, Type = Message.ActorType.Subscriber }))
                            .Union(mime.Bcc.Select(_ => (MailboxAddress)_).Select(_ => new Message.Actor { Name = _.Name, Address = _.Address, Type = Message.ActorType.Logging }))
                        );
                        emails.Add(message);
                    }
                    return emails;
                }
            }
            return null;
        }
    }
}
