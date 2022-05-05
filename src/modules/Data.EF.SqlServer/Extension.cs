using System;
using System.Collections;
using System.Collections.Generic;
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
                var hcBuilder = builder.Services.AddHealthChecks();

                ServiceLifetime lifetime = Options.ServiceLifetime;
                HashSet<Data.DbConnectionSelector> connectionSelectorTable = new();
                var _index = 0;
                foreach (var conn in connections)
                {
                    hcBuilder.AddSqlServer(conn.ConnectionString, name: $"sqlserver-{conn.Name}", tags: new[] { "db", "sql", "sqlserver" });
                    
                    Action<DbContextOptionsBuilder> options = _ =>
                    {
                        _.UseSqlServer(
                            conn.ConnectionString,
                            __ => { 
                                __.EnableRetryOnFailure();
                                #warning TODO use new execution strategy options
                            }
                        );
                    };
                    if (_index++ == 0)
                        builder.Services.AddDbContext<DbContext>(options, lifetime);

                    connectionSelectorTable.Add(new(conn));
                }

                if (_index > 1)
                {
                    var dbContextCollection = Data.DbConnectionSelector.Collection(builder, connectionSelectorTable);
                    var funcWrapper = new DbConnectionFunctionWrapper() { Func = type => (Data.DbConnection)dbContextCollection[type.FullName] };
                    builder.Services.AddSingleton(typeof(DbConnectionFunctionWrapper), funcWrapper);
                }


                builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SqlServer<,>));
            }
        }
    }
}