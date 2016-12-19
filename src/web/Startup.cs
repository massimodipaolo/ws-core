using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace web
{
    public class Startup
    {
        private IHostingEnvironment _env { get; set; }
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables(); //override any config files / user secrets          

            _config = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions().Configure<Configuration>(_config);

            //framework service (Mvc,EF...)
            services.AddMvc();

            // Db repo
            services.AddTransient(typeof(Data.IRepository<>), typeof(Data.Mongo<>));

            // Cache repo
            // https://github.com/esendir/MongoRepository.Cached

            //app service
            services.AddTransient<IMessage, EmailMessage>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.Map("/info", _ => _.Run(async (context) =>
             {
                 var msg = @"
..####...#####...#####............####....####...#####...######.
.##..##..##..##..##..##..........##..##..##..##..##..##..##.....
.######..#####...#####...######..##......##..##..#####...####...
.##..##..##......##..............##..##..##..##..##..##..##.....
.##..##..##......##...............####....####...##..##..######.
................................................................
";
				msg += 
					"\n" +
					$"ApplicationName: {_env.ApplicationName}\n" +
					$"Environment: {_env.EnvironmentName}\n" +
					$"MachineName: {Environment.MachineName}\n" +
					$"ProcessorCount: {Environment.ProcessorCount}\n" ;
				
				await context.Response.WriteAsync(msg);
             }));


        }
    }
}
