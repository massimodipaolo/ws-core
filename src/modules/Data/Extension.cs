using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data
{
    class Extension : Base.Extension
    {
        public override void Execute(WebApplicationBuilder builder, IServiceProvider provider = null)
        {
            base.Execute(builder, null);

            builder.Services
                //.AddTransient(typeof(IRepository<>), typeof(Repository.InMemory<>))
                .AddTransient(typeof(IRepository<,>), typeof(Repository.InMemory<,>));            
        }
    }
}
