using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using core.Extensions.Base;
using System.Collections.Generic;

namespace core.Extensions.StaticFiles
{
    public class OptionsChangeHandler: Base.ConfigurationChangeHandler
    {
        public override void HandleEvent(ConfigurationChangeContext ctx)
        {
            base.HandleEvent(ctx);            
            new Extension().ReloadOptions<List<Options>>(ctx);
        }

    }
}
