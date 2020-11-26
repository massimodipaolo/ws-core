using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.EF.MySql
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            var connections = _options?.Connections;
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
                    hcBuilder.AddMySql(conn.ConnectionString, name: $"mysql-{conn.Name}");

                var _defaultConn = connections.FirstOrDefault().ConnectionString;
                serviceCollection.AddDbContext<AppDbContext>(_ => _.UseMySql(_defaultConn,ServerVersion.AutoDetect(_defaultConn)),_options.ServiceLifetime);
                serviceCollection.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF<,>));
            }
        }
    }
}