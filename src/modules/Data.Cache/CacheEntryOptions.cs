using Microsoft.Extensions.Caching.Distributed;

namespace Ws.Core.Extensions.Data.Cache;

public class CacheEntryOptions : DistributedCacheEntryOptions
{
    public CacheEntryOptions() { }

    public class Expiration
    {
        private static Options.Duration _duration { get; set; } = new Options.Duration() { };
        protected Expiration(Options.Duration? duration = null)
        {
            _duration = duration ?? new Options.Duration() { };
        }
        public static CacheEntryOptions Fast => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Fast) };
        public static CacheEntryOptions Medium => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Medium) };
        public static CacheEntryOptions Slow => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Slow) };
        public static CacheEntryOptions Never => new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_duration.Never) };
    }
}
