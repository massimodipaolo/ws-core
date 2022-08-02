using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data;

class Extension : Base.Extension
{
    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Add(builder, null);
        builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository.InMemory<,>));
    }
}
