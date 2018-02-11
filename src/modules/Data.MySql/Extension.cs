using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace core.Extensions.Data.MySql
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
                serviceCollection.AddDbContext<AppDbContext>(_ => _.UseMySql(connections.FirstOrDefault().ConnectionString));
                serviceCollection.PostConfigure<AppDbContext>(_ => _.Database.EnsureCreated());
                serviceCollection.TryAddTransient(typeof(IRepository<>), typeof(Repository.MySql<>));
            }
        }
    }
}