using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLOG = NLog;

namespace Ws.Core.NLog
{
    public class Startup<TOptions> : Core.Startup<TOptions> where TOptions : class, IAppConfiguration
    {
        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
        {
            
            // logger config                
            var logDb = (NLOG.Targets.DatabaseTarget)NLOG.LogManager.Configuration.FindTargetByName("db");
            if (logDb != null && logDb.ConnectionString.ToString() == "''")
                logDb.ConnectionString = config[$"{Core.Extensions.Base.Configuration.SectionRoot}:assemblies:Ws.Core.Extensions.Data.EF.SqlServer:options:connections:0:connectionString"];
            
            /*
            var logMail = (NLOG.Targets.MailTarget)NLOG.LogManager.Configuration.FindTargetByName("mail");
            if (logMail != null && logMail.SmtpServer.ToString() == "''")
            {
                var smtp = _config.GetSection($"{Core.Extensions.Base.Configuration.SectionRoot}:assemblies:Ws.Core.Extensions.Message:options:senders:0").Get<Core.Extensions.Message.Options.Endpoint>();
                if (smtp != null)
                {
                    logMail.SmtpServer = smtp.Address;
                    logMail.SmtpPort = smtp.Port;
                    logMail.SmtpUserName = smtp.UserName;
                    logMail.SmtpPassword = smtp.Password;
                }
            } 
            */
                        
        }

        public override void Configure(WebApplication app, IOptionsMonitor<TOptions> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime applicationLifetime, ILogger<Ws.Core.Program> logger)
        {
            logger.LogInformation("Start");

            base.Configure(app, appConfigMonitor, extConfigMonitor, applicationLifetime,logger);

            // log shutdown
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Shutdown");
            }
            );
        }
    }
}
