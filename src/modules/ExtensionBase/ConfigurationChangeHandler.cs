using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace core.Extensions.Base {
    public class ConfigurationChangeHandler : IConfigurationChangeEvent
    {
        public int Priority => 0;

        public virtual void HandleEvent(IApplicationBuilder app, Configuration config)
        {
            
        }
    }   
}
