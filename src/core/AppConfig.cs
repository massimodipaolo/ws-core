namespace Ws.Core;

public interface IAppConfiguration
{
    bool DeveloperExceptionPage { get; set; }
}

public class AppConfig : IAppConfiguration
{
    public bool DeveloperExceptionPage { get; set; } = false;
}


