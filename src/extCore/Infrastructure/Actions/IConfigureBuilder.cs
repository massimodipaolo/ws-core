using Microsoft.AspNetCore.Builder;

namespace ExtCore.Infrastructure.Actions;

/// <summary>
/// Describes an action that must be executed inside the ConfigureServices method of a web application's Startup class
/// and might be used by the extensions to register any service inside the DI.
/// </summary>
public interface IConfigureBuilder: IConfigureAction
{
    /// <summary>
    /// Contains any code that must be executed inside the Program class file, before WebApplication Build.
    /// </summary>
    /// <param name="builder">
    /// Will be provided by the ExtCore and might be used to register any service inside the DI.
    /// </param>
    /// <param name="serviceProvider">
    /// Will be provided by the ExtCore and might be used to get any service that is registered inside the DI at this moment.
    /// </param>
    void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null);
}