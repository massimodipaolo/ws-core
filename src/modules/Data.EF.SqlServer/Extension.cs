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
                HashSet<Ws.Core.Extensions.Data.EF.DbContextSelector.Item> serviceSelectorTable = new();
                var _index = 0;
                foreach (var conn in connections.Take(8))
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
                    switch (_index++)
                    {
                        case 0:
                            if (connections.Count() == 1)
                                builder.Services.AddDbContext<DbContext>(options, lifetime);
                            else
                            {
                                builder.Services.AddDbContext<DbContext0>(options, lifetime);
                                serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext0(p.GetService<DbContextOptions<DbContext0>>())));
                            }
                            break;
                        case 1:
                            builder.Services.AddDbContext<DbContext1>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext1(p.GetService<DbContextOptions<DbContext1>>())));
                            break;
                        case 2:
                            builder.Services.AddDbContext<DbContext2>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext2(p.GetService<DbContextOptions<DbContext2>>())));
                            break;
                        case 3:
                            builder.Services.AddDbContext<DbContext3>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext3(p.GetService<DbContextOptions<DbContext3>>())));
                            break;
                        case 4:
                            builder.Services.AddDbContext<DbContext4>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext4(p.GetService<DbContextOptions<DbContext4>>())));
                            break;
                        case 5:
                            builder.Services.AddDbContext<DbContext5>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext5(p.GetService<DbContextOptions<DbContext5>>())));
                            break;
                        case 6:
                            builder.Services.AddDbContext<DbContext6>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext6(p.GetService<DbContextOptions<DbContext6>>())));
                            break;
                        case 7:
                            builder.Services.AddDbContext<DbContext7>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext7(p.GetService<DbContextOptions<DbContext7>>())));
                            break;
                        case 8:
                            builder.Services.AddDbContext<DbContext8>(options, lifetime);
                            serviceSelectorTable.Add(new(conn.ServiceResolver, p => new DbContext8(p.GetService<DbContextOptions<DbContext8>>())));
                            break;
                    }
                }

                if (_index > 1)
                {
                    var dbContextCollection = EF.DbContextSelector.Collection(builder, serviceSelectorTable);
                    var funcWrapper = new DbContextFunctionWrapper() { Func = type => (EF.DbContext)dbContextCollection[type.FullName] };
                    builder.Services.AddSingleton(typeof(DbContextFunctionWrapper), funcWrapper);
                } 


                builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SqlServer<,>));
            }
        }
    }
}