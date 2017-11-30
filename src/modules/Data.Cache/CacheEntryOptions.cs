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
        
        //public static class Expiration
        //{
        //    public static CacheEntryOptions Fast => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EntryProfileExpirationInMinutes.Fast) };
        //    public static CacheEntryOptions Medium => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EntryProfileExpirationInMinutes.Medium) };
        //    public static CacheEntryOptions Slow => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EntryProfileExpirationInMinutes.Slow) };
        //    public static CacheEntryOptions Never => new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EntryProfileExpirationInMinutes.Never) };
        //}
    }
}
