using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Ws.Core.Extensions.Data.FileSystem
{
    public class Extension : Base.Extension
    {
        internal Options Options => GetOptions<Options>();
        public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);
            builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.FileSystem<,>));
        }
    }
}