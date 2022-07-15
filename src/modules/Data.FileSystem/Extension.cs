using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.FileSystem;

public class Extension : Base.Extension
{
    internal Options Options => GetOptions<Options>();
    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Execute(builder, serviceProvider);
        builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.FileSystem<,>));
    }
}