using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace core.Code
{
    public class Utils
    {
        #region DIAGNOSTICS
        public static IEnumerable<Assembly> Assemblies => GetAssemblies();
        private static IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                try
                {
                    var assemblyName = AssemblyLoadContext.GetAssemblyName(module.FileName);                    
                    assemblies.Add(Assembly.Load(assemblyName));
                }
                catch (BadImageFormatException)
                {
                    // ignore native modules
                }
            }
            return assemblies;
        }
        #endregion

    }
}
