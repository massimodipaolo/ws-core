using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache
{
    public interface ICache
    {
        IEnumerable<string> Keys { get; }
        object? Get(string key);
        T? Get<T>(string key);
        void Set(string key, object value);
        void Set(string key, object value, ICacheEntryOptions options);
        void Remove(string key);
        void Clear();
    }

    /// <summary>
    /// Use in DI to map implementation (memory,distributed,sql server...) to type
    /// </summary>
    /// <example>
    /// builder.Services.TryAddSingleton(typeof(ICache<![CDATA[<FooType>]]>), typeof(MemcachedCache<![CDATA[<FooType>]]>));
    /// </example>
    /// <typeparam name="T"></typeparam>
#pragma warning disable S2326 // Unused type parameters should be removed
    public interface ICache<T> : ICache where T : class
#pragma warning restore S2326 // Unused type parameters should be removed
    {
    }
}
