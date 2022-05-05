using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
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

                ServiceLifetime lifetime = ServiceLifetime.Scoped;
                HashSet<Data.DbConnectionSelector> connectionSelectorTable = new();
                var _index = 0;
                foreach (var conn in connections)
                {
                    hcBuilder.AddSqlite(conn.ConnectionString, name: $"sqlite-{conn.Name}", tags: new[] { "db", "sql", "sqlite" });

                    if (_index++ == 0)
                        builder.Services.AddDbContext<DbContext>(_ => _.UseSqlite(conn.ConnectionString), lifetime);

                    connectionSelectorTable.Add(new(conn));
                }

                if (_index > 1)
                {
                    var dbContextCollection = Data.DbConnectionSelector.Collection(builder, connectionSelectorTable);
                    var funcWrapper = new DbConnectionFunctionWrapper() { Func = type => (Data.DbConnection)dbContextCollection[type.FullName] };
                    builder.Services.AddSingleton(typeof(DbConnectionFunctionWrapper), funcWrapper);
                }

                builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SQLite<,>));
            }
        }
    }
}