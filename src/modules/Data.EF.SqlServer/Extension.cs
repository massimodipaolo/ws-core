using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.EF.SqlServer
{
    public class Extension : Base.Extension
    {
        public Options Options => GetOptions<Options>();
        
        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            var connections = Options?.Connections;
            if (connections != null && connections.Any())
            {
                /*
                 serviceCollection.Configure<Options>(_ =>
                {
                    _.Connections = connections;
                });
                */
                var hcBuilder = builder.Services.AddHealthChecks();
                foreach (var conn in connections)
                    hcBuilder.AddSqlServer(conn.ConnectionString, name: $"sqlserver-{conn.Name}", tags: new[] { "db", "sql", "sqlserver" });

                builder.Services.AddDbContext<AppDbContext>(_ => _.UseSqlServer(connections.FirstOrDefault().ConnectionString),Options.ServiceLifetime);
#warning TODO use new execution strategy options
                //builder.Services.AddSqlServer<AppDbContext>(connections.FirstOrDefault().ConnectionString, _ => { _.EnableRetryOnFailure(); /**/ });
                builder.Services.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SqlServer<,>));
            }
        }
    }
}