namespace ExtCore.Infrastructure.Actions;

/// <summary>
/// Describes an action that must be executed inside the Configure method of a web application's Startup class
/// and might be used by the extensions to configure a web application's request pipeline.
/// </summary>
public interface IConfigureAction
{
/// <summary>
/// Priority of the action. The actions will be executed in the order specified by the priority (from smallest to largest).
/// </summary>
int Priority { get; }
}