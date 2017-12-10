using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.Cache
{
    public class CacheEntryOptions : ICacheEntryOptions
    {
        public CacheEntryOptions() { }

        public DateTimeOffset? AbsoluteExpiration { get; set; }

        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        public TimeSpan? SlidingExpiration { get; set; }

        public static class Expiration
        {
            public static CacheEntryOptions Fast => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Fast) };
            public static CacheEntryOptions Medium => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Medium) };
            public static CacheEntryOptions Slow => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Slow) };
            public static CacheEntryOptions Never => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Never) };

            /*
            public static void Set()
            {
                Fast = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Fast) };
                Medium = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Medium) };
                Slow = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Slow) };
                Never = new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Options.EntryExpirationInMinutes.Never) };
            }*/
        }
    }
}
