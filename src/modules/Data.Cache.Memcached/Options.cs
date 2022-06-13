using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.Memcached
{
    public class Options : IOptions
    {
        [Description("Memcached client options")]
        public Enyim.Caching.Configuration.MemcachedClientOptions Client { get; set; }
        [Description("Tier cache expiration in minutes")]
        public Ws.Core.Extensions.Data.Cache.Options.Duration EntryExpirationInMinutes { get; set; } = new();

    }
}