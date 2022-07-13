using Microsoft.Extensions.Caching.Redis;
using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.Redis
{
    public class Options : IOptions, IOptionEntryExpiration
    {
        [Description("Redis client options")]
        public RedisCacheOptions Client { get; set; }
        [Description("Tier cache expiration in minutes")]
        public Ws.Core.Extensions.Data.Cache.EntryExpiration EntryExpirationInMinutes { get; set; } = new();
    }
}