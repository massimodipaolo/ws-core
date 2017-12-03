using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace core.Extensions.StaticFiles
{
    public class OptionsChangeHandler: Base.ConfigurationChangeHandler
    {
        public override void HandleEvent(IApplicationBuilder app, Base.Configuration config)
        {
            base.HandleEvent(app, config);
            new Extension().Reload(app);
        }

    }
}
