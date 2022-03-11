using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Ws.Core.Extensions.Data.EF.SQLite
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            var connections = options?.Connections;
            if (connections != null && connections.Any())
            {
                var hcBuilder = serviceCollection.AddHealthChecks();
                foreach (var conn in connections)
                    hcBuilder.AddSqlite(conn.ConnectionString, name: $"sqlite-{conn.Name}", tags: new[] { "db", "sql", "sqlite" });

                var _defaultConn = connections.FirstOrDefault().ConnectionString;
                serviceCollection.AddDbContext<AppDbContext>(_ => _.UseSqlite(_defaultConn));
                serviceCollection.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SQLite<,>));
            }
        }
    }
}