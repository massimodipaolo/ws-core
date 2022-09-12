using System.Collections.Concurrent;
using System.Reflection;

namespace ExtCore.Infrastructure;

/// <summary>
/// Represents the assembly cache with the mechanism of getting implementations or instances of a given type.
/// This is the global access point to the ExtCore type discovering mechanism.
/// </summary>
public static class ExtensionManager
{
    private static IEnumerable<Assembly> assemblies = Array.Empty<Assembly>();
    private static readonly ConcurrentDictionary<Type, IEnumerable<Type>> types = new();

    /// <summary>
    /// Gets the cached assemblies that have been set by the SetAssemblies method.
    /// </summary>
    public static IEnumerable<Assembly> Assemblies
    {
        get
        {
            return ExtensionManager.assemblies;
        }
    }

    /// <summary>
    /// Sets the assemblies and invalidates the type cache.
    /// </summary>
    /// <param name="assemblies">The assemblies to set.</param>
    public static void SetAssemblies(IEnumerable<Assembly> assemblies)
    {
        ExtensionManager.assemblies = assemblies;
    }

    /// <summary>
    /// Gets the implementations of the type specified by the type parameter and located in the assemblies
    /// filtered by the predicate.
    /// </summary>
    /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
    /// <param name="predicate">The predicate to filter the assemblies.</param>
    /// <param name="useCaching">
    /// Determines whether the type cache should be used to avoid assemblies scanning next time,
    /// when the same type(s) is requested.
    /// </param>
    /// <returns>Found implementations of the given type.</returns>
    public static IEnumerable<Type> GetImplementations<T>(Func<Assembly, bool>? predicate, bool useCaching = false)
    {
        Type type = typeof(T);

        if (useCaching && ExtensionManager.types.ContainsKey(type))
            return ExtensionManager.types[type];

        List<Type> implementations = new();

        foreach (Assembly assembly in ExtensionManager.GetAssemblies(predicate))
            foreach (Type exportedType in assembly.GetExportedTypes())
                if (type.GetTypeInfo().IsAssignableFrom(exportedType) && exportedType.GetTypeInfo().IsClass)
                    implementations.Add(exportedType);

        if (useCaching)
            ExtensionManager.types[type] = implementations;

        return implementations;
    }

    /// <summary>
    /// Gets the new instances of the implementations of the type specified by the type parameter
    /// or empty enumeration if no implementations found.
    /// </summary>
    /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
    /// <param name="useCaching">
    /// Determines whether the type cache should be used to avoid assemblies scanning next time,
    /// when the instance(s) of the same type(s) is requested.
    /// </param>
    /// <returns>The instances of the found implementations of the given type.</returns>
    public static IEnumerable<T> GetInstances<T>(bool useCaching = false)
    {
        return ExtensionManager.GetInstances<T>(null, useCaching, Array.Empty<object>());
    }

    /// <summary>
    /// Gets the new instances (using constructor that matches the arguments) of the implementations
    /// of the type specified by the type parameter and located in the assemblies filtered by the predicate
    /// or empty enumeration if no implementations found.
    /// </summary>
    /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
    /// <param name="predicate">The predicate to filter the assemblies.</param>
    /// <param name="useCaching">
    /// Determines whether the type cache should be used to avoid assemblies scanning next time,
    /// when the instance(s) of the same type(s) is requested.
    /// </param>
    /// <param name="args">The arguments to be passed to the constructors.</param>
    /// <returns>The instances of the found implementations of the given type.</returns>
    public static IEnumerable<T> GetInstances<T>(Func<Assembly, bool>? predicate, bool useCaching = false, params object[] args)
    {
        List<T> instances = new();

        foreach (Type implementation in ExtensionManager.GetImplementations<T>(predicate, useCaching))
            if (implementation != null && !implementation.GetTypeInfo().IsAbstract && Activator.CreateInstance(implementation, args) is T instance && !instances.Contains(instance))
                instances.Add(instance);

        return instances;
    }

    private static IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool>? predicate)
    {
        if (predicate == null)
            return ExtensionManager.Assemblies;

        return ExtensionManager.Assemblies.Where(predicate);
    }
}











