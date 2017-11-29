﻿using Microsoft.Extensions.Caching.Redis;
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
            public RedisCacheOptions RedisOptions { get; set; }            

            public enum Types
            {
                Memory,
                Distributed,
                Redis,
                SqlServer
            }
        }
    }
}