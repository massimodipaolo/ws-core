using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.EF.MySql
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            var connections = options?.Connections;
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
                    hcBuilder.AddMySql(conn.ConnectionString, name: $"mysql-{conn.Name}", tags: new[] { "db", "sql", "mysql" });

                var _defaultConn = connections.FirstOrDefault().ConnectionString;
                builder.Services.AddDbContext<AppDbContext>(_ => _.UseMySql(_defaultConn,ServerVersion.AutoDetect(_defaultConn)),options.ServiceLifetime);
                builder.Services.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.MySql<,>));
            }
        }
    }
}