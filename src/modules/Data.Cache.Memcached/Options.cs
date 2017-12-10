using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.SqlServer;
using System;
using System.Collections.Generic;
using System.Text;
using core.Extensions.Base;

namespace core.Extensions.Data.Cache.Memcached
{
    public class Options : IOptions
    {
        public Enyim.Caching.Configuration.MemcachedClientOptions Client { get; set; }
        public static core.Extensions.Data.Cache.Options.Duration EntryExpirationInMinutes { get; set; }

    }
}