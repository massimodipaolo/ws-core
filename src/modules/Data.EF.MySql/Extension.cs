using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.EF.MySql;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>();
    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Add(builder, serviceProvider);
        _add(builder);
    }

    private void _add(WebApplicationBuilder builder)
    {
        if (options?.Connections?.Any() == true)
        {
            var hcBuilder = builder.Services.AddHealthChecks();

            ServiceLifetime lifetime = options.ServiceLifetime;
            HashSet<Data.DbConnectionSelector> connectionSelectorTable = new();
            var _index = 0;
            foreach (var conn in options.Connections)
            {
                if (!string.IsNullOrEmpty(conn.ConnectionString))
                {
                    hcBuilder.AddMySql(conn.ConnectionString ?? "", name: $"mysql-{conn.Name}", tags: new[] { "db", "sql", "mysql" });

                    Action<DbContextOptionsBuilder> optionBuilder = _ =>
                    {
                        _.UseMySql(conn.ConnectionString, ServerVersion.AutoDetect(conn.ConnectionString));
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

            builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.MySql<,>));
        }
    }
}