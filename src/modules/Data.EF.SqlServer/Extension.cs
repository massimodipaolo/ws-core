using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.EF.SqlServer
{
    public class Extension : Base.Extension
    {
        public Options _options => GetOptions<Options>();

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
                serviceCollection.AddDbContext<AppDbContext>(_ => _.UseSqlServer(connections.FirstOrDefault().ConnectionString),_options.ServiceLifetime);                
                serviceCollection.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.EF.SqlServer<,>));
            }
        }
    }
}