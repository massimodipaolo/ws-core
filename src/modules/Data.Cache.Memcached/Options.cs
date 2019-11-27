using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.Memcached
{
    public class Options : IOptions
    {
        public Enyim.Caching.Configuration.MemcachedClientOptions Client { get; set; }
        public static Ws.Core.Extensions.Data.Cache.Options.Duration EntryExpirationInMinutes { get; set; }

    }
}