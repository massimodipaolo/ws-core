using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace core.Extensions.Base
{
    public class Util
    {
        public Util()
        {
        }

        public static IEnumerable<Type> GetAllTypesOf<T>()
        {
            return GetAllTypesOf(typeof(T));
        }

        public static IEnumerable<Type> GetAllTypesOf(Type type)
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            var types = runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface);

            return types;
        }

    }
}
