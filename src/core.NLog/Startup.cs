using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLOG = NLog;

namespace Ws.Core.NLog
{
    public class Startup<TOptions> : Core.Startup<TOptions> where TOptions : class, IAppConfiguration
    {
        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory loggerFactory) : base(hostingEnvironment, configuration, loggerFactory)
        {
            
            // logger config    
            /*
            var logDb = (NLOG.Targets.DatabaseTarget)NLOG.LogManager.Configuration.FindTargetByName("db");
            if (logDb != null && logDb.ConnectionString.ToString() == "''")
                logDb.ConnectionString = _config[$"{core.Extensions.Base.Configuration.SectionRoot}:assemblies:Ws.Core.Extensions.Data.EF.SqlServer:options:connections:0:connectionString"];
            
            var logMail = (NLOG.Targets.MailTarget)NLOG.LogManager.Configuration.FindTargetByName("mail");
            if (logMail != null && logMail.SmtpServer.ToString() == "''")
            {
                var smtp = _config.GetSection($"{core.Extensions.Base.Configuration.SectionRoot}:assemblies:Ws.Core.Extensions.Message:options:senders:0").Get<core.Extensions.Message.Options.Endpoint>();
                if (smtp != null)
                {
                    logMail.SmtpServer = smtp.Address;
                    logMail.SmtpPort = smtp.Port;
                    logMail.SmtpUserName = smtp.UserName;
                    logMail.SmtpPassword = smtp.Password;
                }
            }            
            */

            // log app start
            _logger.CreateLogger<Startup<TOptions>>().LogInformation("Start");
        }

        public override void Configure(IApplicationBuilder app, IOptionsMonitor<TOptions> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IApplicationLifetime applicationLifetime)
        {
            base.Configure(app, appConfigMonitor, extConfigMonitor, applicationLifetime);

            // log shutdown
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _logger.CreateLogger<Startup<TOptions>>().LogInformation("Shutdown");
            }
            );
        }
    }
}
