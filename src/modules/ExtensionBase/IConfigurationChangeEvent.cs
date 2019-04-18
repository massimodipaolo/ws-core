using ExtCore.Events;

namespace Ws.Core.Extensions.Base
{
    public interface IConfigurationChangeEvent : IEventHandler<ConfigurationChangeContext>
    {
    }
}
