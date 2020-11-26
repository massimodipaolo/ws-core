using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Ws.Core.Extensions.Base
{
    public class Util
    {
        private static IEnumerable<Type>  _allTypes { get; set; }
        public Util()
        {
        }

        private static IEnumerable<Type> GetAllTypes()
        {
            if (_allTypes == null)
            {
                var platform = Environment.OSVersion.Platform.ToString();
                var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

                _allTypes = runtimeAssemblyNames
                    .Select(Assembly.Load)
                    .SelectMany(a => {
                        try
                        {
                            return a.ExportedTypes;
                        }
                        catch
                        {
                            return Array.Empty<Type>();
                        }
                    })?.ToList();
            }
            return _allTypes;
        }

        public static IEnumerable<Type> GetAllTypesOf<T>() where T : class 
            => GetAllTypesOf(typeof(T));

        public static IEnumerable<Type> GetAllTypesOf(Type type)
            => 
                GetAllTypes()?
                .Where(t => t != null && type.IsAssignableFrom(t) && !t.IsInterface);

    }
}
