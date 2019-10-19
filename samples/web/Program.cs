using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System;
using System.Threading.Tasks;

namespace web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("Init program");
                var host = Ws.Core.Program.WebHostBuilder(args, typeof(Program).Assembly, typeof(Startup))
                .UseNLog();
                await host.Build().RunAsync();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Stopped program");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
