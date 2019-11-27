using Microsoft.Extensions.Caching.SqlServer;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.SqlServer
{
    public class Options : IOptions
    {
        public SqlServerCacheOptions Client { get; set; }
        public static Ws.Core.Extensions.Data.Cache.Options.Duration EntryExpirationInMinutes { get; set; }

    }
}