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
        
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            var connections = Options?.Connections;
            if (connections != null && connections.Any())
            {
                /*
                 serviceCollection.Configure<Options>(_ =>
                {
                    _.Connections = connections;
                });

                */
                var hcBuilder = serviceCollection.AddHealthChecks();
                foreach (var conn in connections)
                    hcBuilder.AddSqlServer(conn.ConnectionString, name: $"sqlserver-{conn.Name}");

                serviceCollection.AddDbContext<AppDbContext>(_ => _.UseSqlServer(connections.FirstOrDefault().ConnectionString),Options.ServiceLifetime);                
                serviceCollection.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SqlServer<,>));
            }
        }
    }
}