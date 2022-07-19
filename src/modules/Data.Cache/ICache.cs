using Microsoft.Extensions.Caching.Distributed;

namespace Ws.Core.Extensions.Data.Cache;

public interface ICache: IDistributedCache
{
    IEnumerable<string> Keys { get; }
    IExpirationTier ExpirationTier { get; }
    T? Get<T>(string key);
    Task<T?> GetAsync<T>(string key);
    Task SetObjectAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken token = default);
    void Clear();
    Task ClearAsync(CancellationToken token = default);
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

