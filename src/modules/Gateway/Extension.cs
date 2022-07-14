using Microsoft.AspNetCore.Builder;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Ws.Core.Extensions.Gateway;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>() ?? new Options();

    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        if (options.Ocelot != null)
        {
            var config = builder.Configuration.GetSection($"{ConfigSectionPathOptions}:{nameof(Options.Ocelot)}");
            builder.Services.AddOcelot(config);
        }
    }

    public override void Execute(WebApplication app)
    {
        if (options.Ocelot != null)
            if (string.IsNullOrEmpty(options.MapWhen))
                app.UseOcelot().Wait();
            else
            {
                var regex = new System.Text.RegularExpressions.Regex(options.MapWhen, System.Text.RegularExpressions.RegexOptions.Compiled);
                app.MapWhen(
                    (ctx) => regex.IsMatch(ctx.Request.Path.Value ?? ""), 
                    _ => _.UseOcelot().Wait()
                    );
            }
    }
}
