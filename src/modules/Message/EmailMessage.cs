using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.Message
{
    public class EmailMessage : IMessage
    {
        private ILogger<IMessage> logger { get; set; }
        private IMessageConfiguration config { get; set; }
        public EmailMessage(ILogger<IMessage> logger, IMessageConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }

        /*
        public class EmailMessageModel
        {
            public bool IsHtml { get; set; } = true;
        }
        */

        public async Task SendAsync(Message message, bool throwException = false)
        {
            var sender = config.Senders.FirstOrDefault();
            if (sender != null && !string.IsNullOrEmpty(sender.Address))
            {
                var mime = new MimeMessage();
                mime.From.Add(new MailboxAddress(message.Sender.Name, message.Sender.Address));
                mime.To.AddRange(message.Recipients.Where(_ => _.Type == Message.ActorType.Primary).Select(x => new MailboxAddress(x.Name, x.Address)));
                mime.Cc.AddRange(message.Recipients.Where(_ => _.Type == Message.ActorType.Subscriber).Select(x => new MailboxAddress(x.Name, x.Address)));
                mime.Bcc.AddRange(message.Recipients.Where(_ => _.Type == Message.ActorType.Logging).Select(x => new MailboxAddress(x.Name, x.Address)));

                mime.Subject = message.Subject;

                // msg properties
                //EmailMessageModel model = message.Arguments != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<EmailMessageModel>(Newtonsoft.Json.JsonConvert.SerializeObject(message.Arguments)) : new EmailMessageModel();

                var body = new TextPart(string.IsNullOrEmpty(message.Format) || message.Format == "html" ? TextFormat.Html : TextFormat.Plain)
                {
                    Text = message.Content
                };

                switch (message.Priority) {
                    case MessagePriority.Low:
                        mime.Importance = MessageImportance.Low;
                        break;
                    case MessagePriority.Normal:
                        mime.Importance = MessageImportance.Normal;
                        break;
                    case MessagePriority.High:
                        mime.Importance = MessageImportance.High;
                        break;
                };

                var attachments = message.Attachments?.Where(_ => _.Content.Length > 0);
                if (attachments != null && attachments.Any())
                {
                    var multipart = new Multipart("mixed")
                    {
                        body
                    };

                    foreach (var attachment in attachments)
                    {
                        multipart.Add(new MimePart()
                        {
                            FileName = attachment.Name ?? Guid.NewGuid().ToString(),
                            Content = new MimeContent(new System.IO.MemoryStream(attachment.Content)),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64
                        });
                    }

                    mime.Body = multipart;
                }
                else
                    mime.Body = body;

                using var client = new MailKit.Net.Smtp.SmtpClient();
                try
                {
                    if (sender.SkipCertificateValidation)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(sender.Address, sender.Port == 0 ? 25 : sender.Port, sender.EnableSsl ? MailKit.Security.SecureSocketOptions.Auto : MailKit.Security.SecureSocketOptions.None);

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (!string.IsNullOrEmpty(sender.UserName) && !string.IsNullOrEmpty(sender.Password))
                        await client.AuthenticateAsync(sender.UserName, sender.Password).ConfigureAwait(false);

                    await client.SendAsync(mime).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is MailKit.Security.SslHandshakeException || ex is System.Net.Sockets.SocketException || ex is MailKit.ProtocolException)
                {
                    logger.LogError(ex, "Smtp connection error");
                    if (throwException) throw;
                }
                catch (Exception ex) when (ex is MailKit.Security.AuthenticationException || ex is MailKit.Security.SaslException)
                {
                    logger.LogError(ex, "Smtp authentication error");
                    if (throwException) throw;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Send email error");
                    if (throwException) throw;
                }
                finally
                {
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
            }
        }

        public async Task<IEnumerable<Message>> ReceiveAsync()
        {
            var receiver = config.Receivers.FirstOrDefault();
            if (receiver != null && !string.IsNullOrEmpty(receiver.Address))
            {
                using var client = new MailKit.Net.Pop3.Pop3Client();
                await client.ConnectAsync(receiver.Address, receiver.Port == 0 ? 110 : receiver.Port).ConfigureAwait(false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                if (!string.IsNullOrEmpty(receiver.UserName) && !string.IsNullOrEmpty(receiver.Password))
                    await client.AuthenticateAsync(receiver.UserName, receiver.Password).ConfigureAwait(false);
                List<Message> emails = new ();
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
            return null;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var smtp = config.Senders?.FirstOrDefault();
            if (smtp != null)
            {
                if (smtp.Port == 0) smtp.Port = 25;
                var options = new HealthChecks.Network.SmtpHealthCheckOptions()
                {
                    Host = smtp.Address,
                    Port = smtp.Port,
                    AllowInvalidRemoteCertificates = smtp.SkipCertificateValidation,
                    ConnectionType = 
                        smtp.EnableSsl ? HealthChecks.Network.Core.SmtpConnectionType.SSL : 
                        (new int[] { 465, 587, 25 }.Any(_ => _ == smtp.Port) ? 
                            HealthChecks.Network.Core.SmtpConnectionType.AUTO : 
                            HealthChecks.Network.Core.SmtpConnectionType.PLAIN)
                };
                if (!string.IsNullOrEmpty(smtp.UserName))
                    options.LoginWith(smtp.UserName, smtp.Password);
                
                var result = await new HealthChecks.Network.SmtpHealthCheck(options).CheckHealthAsync(context, cancellationToken);
                Dictionary<string, object> data = new()
                {
                    { nameof(smtp.Address), smtp.Address },
                    { nameof(smtp.Port), smtp.Port }
                };
                return new HealthCheckResult(result.Status,result.Description,result.Exception,data);
            };
            return await Task.FromResult(HealthCheckResult.Healthy());
        }      
    }
}
