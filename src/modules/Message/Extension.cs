using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Message;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>();
    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Add(builder, serviceProvider);
        _add(builder);
    }

    private void _add(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddCheck<EmailMessage>("message-email", tags: new[] { "message", "smtp", "email" });
        builder.Services.AddSingleton<IMessageConfiguration>(options);
        builder.Services.AddTransient<IMessage, EmailMessage>();
    }
}
