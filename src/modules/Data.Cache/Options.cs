using Microsoft.Extensions.Caching.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Base
{
    public partial class Options
    {
        public DataCacheOptions DataCache { get; set; }
        public class DataCacheOptions
        {
            public Types Type { get; set; }
            public enum Types
            {
                Memory,
                Distributed,
                Redis,
                SqlServer
            }

            public RedisCacheOptions RedisOptions { get; set; }

            public EntryExpirationInMinutes EntryProfileExpirationInMinutes { get; set; }
            public class EntryExpirationInMinutes
            {
                public int Fast { get; set; } = 10;
                public int Medium { get; set; } = 60;
                public int Slow { get; set; } = 240;
                public int Never { get; set; } = 1440;
            }

        }
    }
}