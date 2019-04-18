using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache
{
    public interface ICacheEntryOptions
    {
        //
        // Summary:
        //     Gets or sets an absolute expiration date for the cache entry.
        DateTimeOffset? AbsoluteExpiration { get; set; }

        //
        // Summary:
        //     Gets or sets an absolute expiration time, relative to now.
        TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
                //
        // Summary:
        //     Gets or sets how long a cache entry can be inactive (e.g. not accessed) before
        //     it will be removed. This will not extend the entry lifetime beyond the absolute
        //     expiration (if set).
        TimeSpan? SlidingExpiration { get; set; }
    }
}
