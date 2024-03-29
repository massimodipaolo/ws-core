﻿using Microsoft.Extensions.Options;

namespace Ws.Core.Extensions.Data.Cache;

public class MemoryCache : DistributedCache
{
    public MemoryCache(IOptions<Microsoft.Extensions.Caching.Memory.MemoryDistributedCacheOptions> options, IExpirationTier<MemoryCache> expirationTier)
        :base(new Microsoft.Extensions.Caching.Distributed.MemoryDistributedCache(options), expirationTier) { }
}
public class MemoryCache<T> : MemoryCache, ICache<T> where T : class
{
    public MemoryCache(IOptions<Microsoft.Extensions.Caching.Memory.MemoryDistributedCacheOptions> options, IExpirationTier<MemoryCache> expirationTier)
        : base(options, expirationTier) { }
}
