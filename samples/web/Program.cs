using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using System;

namespace web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            var host = Ws.Core.Program.WebHostBuilder(args, typeof(Program).Assembly)
                .UseStartup<Startup>()
                .UseNLog()
                .Build();

            try
            {
                host.Run();
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
