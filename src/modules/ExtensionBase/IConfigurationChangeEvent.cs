using System;
using ExtCore.Events;
using Microsoft.AspNetCore.Builder;

namespace core.Extensions.Base
{
    public interface IConfigurationChangeEvent : IEventHandler<IApplicationBuilder, Configuration>
    {
    }
}
