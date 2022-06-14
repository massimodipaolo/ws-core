using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Ws.Core.Extensions.Base;
using System.Collections.Generic;

namespace Ws.Core.Extensions.StaticFiles
{
    public class OptionsChangeHandler: Base.ConfigurationChangeHandler
    {
        public override void HandleEvent(ConfigurationChangeContext ctx)
        {
            base.HandleEvent(ctx);
            var ext = new Extension();
            var options = ext.ReloadOptions<Options>();
            if (options != null) {
                if (ctx.Configuration.EnableShutDownOnChange)
                    ctx.Lifetime.StopApplication();
            }
        }

    }
}
