using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace core.Extensions.Message
{
    public class Extension: Base.Extension
    {
        public override IEnumerable<KeyValuePair<int, Action<IServiceCollection>>> ConfigureServicesActionsByPriorities
        {
            get
            {
                var d = new Dictionary<int, Action<IServiceCollection>>();
                d[Priority] = service => service.AddTransient<IMessage,EmailMessage>();
                return d;
            }
        }
    }
}
