using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.Data.Cache
{
    public class MemoryCache : DistributedCache
    {
        public MemoryCache(IOptions<Microsoft.Extensions.Caching.Memory.MemoryDistributedCacheOptions> options)
            : base(new Microsoft.Extensions.Caching.Distributed.MemoryDistributedCache(options)) { }
    }
    public class MemoryCache<T> : DistributedCache<T> where T : class
    {
        public MemoryCache(IOptions<Microsoft.Extensions.Caching.Memory.MemoryDistributedCacheOptions> options)
            : base(new Microsoft.Extensions.Caching.Distributed.MemoryDistributedCache(options)) { }
    }
}
