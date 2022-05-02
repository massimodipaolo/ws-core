using Microsoft.AspNetCore.Builder;
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

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            var connections = options?.Connections;
            if (connections != null && connections.Any())
            {
                var hcBuilder = builder.Services.AddHealthChecks();
                foreach (var conn in connections)
                {
                    hcBuilder.AddSqlite(conn.ConnectionString, name: $"sqlite-{conn.Name}", tags: new[] { "db", "sql", "sqlite" });
                    builder.Services.AddDbContext<SQLite.DbContext>(_ => _.UseSqlite(conn.ConnectionString));
                }
                builder.Services.PostConfigure<SQLite.DbContext>(_ => _.Database.EnsureCreated());
                builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SQLite<,>));
            }
        }
    }
}