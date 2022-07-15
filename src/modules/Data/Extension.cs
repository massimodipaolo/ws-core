using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data;

class Extension : Base.Extension
{
    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Execute(builder, null);

        builder.Services
            .AddTransient(typeof(IRepository<,>), typeof(Repository.InMemory<,>));            
    }
}
