using ExtCore.Events;

namespace core.Extensions.Base
{
    public interface IConfigurationChangeEvent : IEventHandler<ConfigurationChangeContext>
    {
    }
}
