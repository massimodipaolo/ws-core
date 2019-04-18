using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using NLOG = NLog;

namespace Ws.Core.NLog
{
    public class Program
    {
        private const string _defaultNLogConfigFile = "NLog.config";
        private static Type _startupConfigurationType { get; set; } = typeof(Startup<Core.AppConfig>);
        private static NLOG.Config.LoggingConfiguration _NLogConfiguration { get; set; }
        private static string _NLogConfigFile { get; set; } = _defaultNLogConfigFile;        
        public static void Main(string[] args) 
        {
            var loggerFactory = new NLOG.LogFactory();
            if (_NLogConfiguration != null)
                loggerFactory = NLOG.Web.NLogBuilder.ConfigureNLog(_NLogConfiguration);
            else                
                loggerFactory = NLOG.Web.NLogBuilder.ConfigureNLog(_NLogConfigFile);
            
            var logger = loggerFactory.GetCurrentClassLogger();

            var host = Core.Program.WebHostBuilder(args, typeof(Program).Assembly)                
                .UseStartup(_startupConfigurationType)
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
                NLOG.LogManager.Shutdown();
            }

        }

        /// <summary>
        /// Configure with default Startup options configuration
        /// </summary>
        public static void Configure(string NLogConfigFile = _defaultNLogConfigFile)
        {
            Configure<Core.AppConfig>(NLogConfigFile);
        }

        /// <summary>
        /// Configure with default Startup options configuration
        /// </summary>
        public static void Configure(NLOG.Config.LoggingConfiguration loggingConfiguration)
        {
            Configure<Core.AppConfig>(loggingConfiguration);
        }

        /// <summary>
        /// Configure Startup options Type: IAppConfiguration;        
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="NLogConfigFile">Set the NLog config file relative path</param>
        public static void Configure<TOptions>(string NLogConfigFile = _defaultNLogConfigFile) where TOptions : class, IAppConfiguration
        {
            SetStartupConfiguration<TOptions>();
            _NLogConfigFile = NLogConfigFile;
        }

        /// <summary>
        /// Configure Startup options Type: IAppConfiguration;        
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="loggingConfiguration">Provide a LoggingConfiguration</param>
        public static void Configure<TOptions>(NLOG.Config.LoggingConfiguration loggingConfiguration) where TOptions : class, IAppConfiguration
        {            
            SetStartupConfiguration<TOptions>();
            _NLogConfiguration = loggingConfiguration;
        }

        private static void SetStartupConfiguration<TOptions>() where TOptions : class, IAppConfiguration
        {
            _startupConfigurationType = typeof(Startup<TOptions>);
        }

    }
}
