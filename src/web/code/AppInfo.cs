using Ws.Core.Extensions.Data.Cache;
using Ws.Core.Extensions.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ws.bom.oven.web.code;

public class AppInfo : Ws.Core.AppInfo<code.AppConfig>
{
    public static ICache<T>? Cache<T>() where T : class => ServiceProvider?.GetService<ICache<T>>();
    public static async Task Init()
    {
        // Init gateway
        try
        {
            services.PayloadCms._gateway = Gateway<AppConfig.GatewayConfig.PayloadCmsConfig>();         
        }
        catch (Exception ex)
        {
            Logger<AppInfo>()?.LogError(ex, "Error init gateway");
        }

        // Preload
        var tasks = new List<Action>
            {
                () => _ = Task.CompletedTask,
                async () => await Task.FromResult<bool>(true)
            };
        await Task.Run(() => Parallel.ForEach(tasks, task => task()));
    }

    public static TConfig Gateway<TConfig>(int numRetry = 0) where TConfig : class, AppConfig.IGatewayBase
    {
        try
        {
            var _section = "appconfig:gateway:";
            var _property = typeof(TConfig).Name.Replace("Config", "");
            var gateway = Ws.Core.Shared.Serialization.Util.As<TConfig>(
                typeof(AppConfig.GatewayConfig).GetProperty(_property)?.GetValue(AppConfig.Value?.Gateway) ??
                AppInfo.Config?.GetSection($"{_section}{_property}")?.Get<TConfig>()
                )
                ?? default;
            return gateway;
        }
        catch (Exception ex)
        {
            if (numRetry < 10)
            {
                numRetry++;
                Task.Delay(TimeSpan.FromSeconds(Math.Max(2 * numRetry, 10))).Wait();
                Logger<AppInfo>()?.LogInformation(ex, "Trying getting Gateway configuration {name}. Retry {numRetry}", typeof(TConfig).Name, numRetry);
                return Gateway<TConfig>(numRetry);
            }
            Logger<AppInfo>()?.LogError(ex, "Error getting Gateway configuration {name}", typeof(TConfig).Name);
            throw;
        }
    }
}
