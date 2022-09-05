# Message

#### Table of Contents

1. [Description](#description)
   - [`Message`](#description-message)
   - [`IMessage`](#description-imessage)
     - [`SendAsync`](#description-imessage-send-async)
     - [`ReceiveAsync`](#description-imessage-receive-async)
   - [`EmailMessage`](#description-email-message)
1. [Setup](#setup)
   - [Requirements](#setup-requirements)
   - [Configuration](#setup-configuration)
1. [Usage](#usage)
1. [Limitations](#limitations)
1. [Development](#development)

## <a id="description"></a>Description

The `Message` module install and configures a message service for sending/receiving messages.

It exposes a base class representation of a message instance, a generic interface for the message service and a default message service implementation that sends/receives e-mail via `SMTP`/`POP3` endpoints using [MailKit](https://github.com/jstedfast/MailKit).

### <a id="description-message"></a>`Message`

Class that represents the message instance used by the `IMessage` interface methods.

- **Sender**: `Actor` instance of the sender.
- **Recipients**: list of `Actor` instances of the recipients.
- **Subject**: message subject.
- **Content**: message content.
- **Priority**: `Low`|`Normal`|`High`
- **Format**: implementation-specific message format.
- **Attachments**: list of `Attachment` instances.
- **Arguments**: implementation-specific properties.

#### `Actor`

- **Name**: actor name.
- **Address**: actor address.
- **Type**: `Primary`|`Subscriber`|`Logging`

#### `Attachment`

- **Name**: attachment name.
- **Content**: attachment content in bytes.

### <a id="description-imessage"></a>`IMessage`

Generic interface of the message service.

#### <a id="description-imessage-send-async"></a>`SendAsync`

```csharp
Task SendAsync(Message message, bool throwException = false);
```

#### <a id="description-imessage-receive-async"></a>`ReceiveAsync`

```csharp
Task<IEnumerable<Message>> ReceiveAsync();
```

### <a id="description-email-message"></a>`EmailMessage`

Default implementation of the `IMessage` interface:

- It's automatically registered by the module setup.
- Sends messages using a configured `SMTP` endpoint sender.
- Receives messages using a configured `POP3` endpoint receiver.
- Adds a specific `HealthCheck` implementation to load with the `HealthChecks` module.

## <a id="setup"></a>Setup

You can install the `Message` module adding the reference in your `Core` application and configure it in the `ext-settings.json` file.

### <a id="setup-requirements"></a>Requirements

No other modules are required.

### <a id="setup-configuration"></a>Configuration

You need to define 2 options:

1. **senders**: list of sender endpoints.
1. **receivers**: list of receiver endpoints.

#### Endpoint configuration options

- **address**: endpoint address.
- **port**: endpoint port.
- **userName**: authentication username.
- **password**: authentication password.
- **enableSsl** (_default_: `false`): indicates that the connection should use SSL.
- **skipCertificateValidation** (_default_: `false`): skips the server certificate validation. ⚠️Only available in DEBUG mode!

> **Note**: `userName` and `password` are optional in the default `EmailMessage` service. If not provided no authentication will be provided to the `SMTP`/`POP3` endpoints.

#### Configuration example

```json
{
  "extConfig": {
    "assemblies": {
      "Ws.Core.Extensions.Message": {
        "priority": 100,
        "options": {
          "senders": [
            {
              "address": "127.0.0.1",
              "port": 2025
            }
          ],
          "receivers": [
            {
              "address": "127.0.0.1",
              "port": 2110,
              "userName": "test.user@mail.local",
              "password": "testpwd",
              "enableSsl": true,
              "skipCertificateValidation": true
            }
          ]
        }
      }
    }
  }
}
```

## <a id="usage"></a>Usage

Use the `IMessage` interface in your code, the dependency injection engine will directly resolve the only implementation.

#### Usage example

```csharp
public class TestMessenger
{
    IMessage _service;

    public TestMessenger(IMessage service)
    {
        _service = service;
    }

    public async Task<string> TestSend(string sender, string recipient)
    {
        var message = new Message()
        {
            Sender = new Message.Actor()
            {
                Address = sender,
                Name = sender
            },
            Recipients = new Message.Actor[]
            {
                new()
                {
                    Address = recipient,
                    Name = recipient,
                    Type = Message.ActorType.Primary
                }
            },
            Subject = $"Subject {sender.Split('@')[1]} {DateTime.UtcNow.ToShortTimeString()}",
            Content = "Message content",
            Arguments = new { arg1 = "foo", arg2 = "bar" },
            Format = "html"
        };

        message.Attachments = new List<Message.Attachment>()
        {
            new()
            {
                Name = "message.txt",
                Content = System.Text.Encoding.ASCII.GetBytes(JsonSerializer.Serialize(message))
            }
        };

        await _service.SendAsync(message, throwException: true);

        return JsonSerializer.Serialize(message);
    }

    public async Task<string> TestReceive()
    {
        IEnumerable<Message> messages = await _service.ReceiveAsync();

        return JsonSerializer.Serialize(messages);
    }
}
```

## <a id="limitations"></a>Limitations

⚠️Be aware that the default `EmailMessage` service does not implement multiple senders/receivers configurations. If you configure multiple senders/receivers no error will be thrown but only the first one of each collection will be used.

## <a id="development"></a>Development

Development note, contribution info, licensing to be defined.
