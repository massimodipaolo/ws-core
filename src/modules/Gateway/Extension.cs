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
            /*
            builder.Host.ConfigureHostConfiguration(_ =>
            {
                //_.AddOcelot(builder.Environment); // ocelot.global.json + merge ocelot.{env}.json => ocelot.json            
                string config = System.Text.Json.JsonSerializer.Serialize<Ocelot.Configuration.File.FileConfiguration>(options.Ocelot);
                File.WriteAllText("ocelot.json", config);
                _.AddJsonFile("ocelot.json", optional: false, reloadOnChange: false);
            });
            */
            var config = builder.Configuration.GetSection($"{ConfigSectionPathOptions}:{nameof(Options.Ocelot)}");
            //Ocelot.Configuration.File.FileConfiguration opt = config.Get<Ocelot.Configuration.File.FileConfiguration>();
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
