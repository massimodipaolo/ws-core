using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Ws.Core.Extensions.Data.Cache;

public interface IExpirationTier
{
    DistributedCacheEntryOptions NoCache { get; }
    DistributedCacheEntryOptions Fast { get; }
    DistributedCacheEntryOptions Medium { get; }
    DistributedCacheEntryOptions Slow { get; }
    DistributedCacheEntryOptions Never { get; }
}

/// <summary>
/// Use in DI to map implementation (memory,distributed,sql server...) to type
/// </summary>
/// <example>
/// builder.Services.TryAddSingleton(typeof(IExpirationTier<![CDATA[<FooType>]]>), typeof(ExpirationTier<![CDATA[<FooType>]]>));
/// </example>
/// <typeparam name="T"></typeparam>
public interface IExpirationTier<TCache> : IExpirationTier where TCache : ICache { }

public class ExpirationTier : IExpirationTier
{
    private EntryExpiration? _duration { get; set; }
    public readonly static DistributedCacheEntryOptions NoCacheValue = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1) };
    public ExpirationTier(EntryExpiration? duration = null) {
        _duration = duration ?? new EntryExpiration() { };
    }
    static readonly Func<double?, DistributedCacheEntryOptions> _tier = (minutes) => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes ?? 1) };
    public DistributedCacheEntryOptions NoCache => NoCacheValue;
    public DistributedCacheEntryOptions Fast => _tier(_duration?.Fast);
    public DistributedCacheEntryOptions Medium => _tier(_duration?.Medium);
    public DistributedCacheEntryOptions Slow => _tier(_duration?.Slow);
    public DistributedCacheEntryOptions Never => _tier(_duration?.Never);
}

public class ExpirationTier<TCache>: ExpirationTier, IExpirationTier<TCache> where TCache : ICache
{
    public ExpirationTier(EntryExpiration? duration = null) : base(duration) {}
}