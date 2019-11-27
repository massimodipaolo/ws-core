using Microsoft.Extensions.Caching.Redis;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.Redis
{
    public class Options : IOptions
    {
        public RedisCacheOptions Client { get; set; }
        public static Ws.Core.Extensions.Data.Cache.Options.Duration EntryExpirationInMinutes { get; set; }
    }
}