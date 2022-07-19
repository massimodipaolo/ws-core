using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.EF.SqlServer;

public class Extension : Base.Extension
{
    public Options Options => GetOptions<Options>();
    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Execute(builder, serviceProvider);

        if (Options?.Connections?.Any() == true)
        {
            var hcBuilder = builder.Services.AddHealthChecks();

            ServiceLifetime lifetime = Options.ServiceLifetime;
            HashSet<Data.DbConnectionSelector> connectionSelectorTable = new();
            var _index = 0;
            foreach (var conn in Options.Connections)
            {
                if (!string.IsNullOrEmpty(conn.ConnectionString))
                {
                    hcBuilder.AddSqlServer(conn.ConnectionString, name: $"sqlserver-{conn.Name}", tags: new[] { "db", "sql", "sqlserver" });

                    Action<DbContextOptionsBuilder> optionBuilder = _ =>
                    {
                        _.UseSqlServer(conn.ConnectionString);
                    };
                    if (_index++ == 0)
                        builder.Services.AddDbContext<DbContext>(optionBuilder, lifetime);

                    connectionSelectorTable.Add(new(conn));
                }
            }

            if (_index > 1)
            {
                var dbContextCollection = Data.DbConnectionSelector.Collection(connectionSelectorTable);
                var funcWrapper = new DbConnectionFunctionWrapper() { Func = type => (DbConnection)(dbContextCollection[type?.FullName ?? ""] ?? connectionSelectorTable.First()) };
                builder.Services.AddSingleton(typeof(DbConnectionFunctionWrapper), funcWrapper);
            }


            builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SqlServer<,>));
        }
    }
}