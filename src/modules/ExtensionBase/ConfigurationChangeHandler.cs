namespace core.Extensions.Base
{
    public class ConfigurationChangeHandler : IConfigurationChangeEvent
    {
        public int Priority => 0;

        public virtual void HandleEvent(ConfigurationChangeContext ctx)
        {

        }
    }
}
