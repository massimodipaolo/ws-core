using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using System.Threading;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.Base
{
    public class Util
    {
        private static IEnumerable<Assembly> _allAssemblies { get; set; }
        private static readonly Locker _mutexAssemblies = new();
        private static IEnumerable<Type> _allTypes { get; set; }
        private static readonly Locker _mutexTypes = new();
        public Util()
        {
        }

        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            if (_allAssemblies == null)
                using (_mutexAssemblies.Lock())
                    if (_allAssemblies == null)
                    {
                            var platform = Environment.OSVersion.Platform.ToString();
                            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);
                            _allAssemblies = runtimeAssemblyNames
                            .Select(Assembly.Load)
                            .ToList();                       
                    }
            return _allAssemblies;
        }

        private static IEnumerable<Type> GetAllTypes()
        {
            if (_allTypes == null)
                using (_mutexTypes.Lock())
                    if (_allTypes == null)
                    {
                        _allTypes = GetAllAssemblies()
                            .SelectMany(a =>
                            {
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

        public static Type GetType(string typeFullName)
            =>
                GetAllTypes()?
                .Where(t => t.FullName == typeFullName)?
                .FirstOrDefault();

        public sealed class Locker : IDisposable
        {
            private SemaphoreSlim _semaphoreSlim { get; set; }

            public Locker()
            {
                _locker(1);
            }

            public Locker(int maxCount)
            {
                _locker(maxCount);
            }

            private void _locker(int maxCount)
            {
                _semaphoreSlim = new SemaphoreSlim(1, maxCount);
            }

            public Locker Lock()
            {
                _semaphoreSlim.Wait();
                return this;
            }

            public async Task<Locker> LockAsync()
            {
                await _semaphoreSlim.WaitAsync();
                return this;
            }

            public void Dispose()
            {
                _semaphoreSlim.Release();
            }
        }

    }
}
