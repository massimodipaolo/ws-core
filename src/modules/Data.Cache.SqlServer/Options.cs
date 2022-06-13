using Microsoft.Extensions.Caching.SqlServer;
using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.SqlServer
{
    public class Options : IOptions
    {
        [Description("SqlServer cache client")]
        [DefaultValue("ConnString: local Trusted; Tbl: dbo.Entry")]
        public SqlServerCacheOptions Client { get; set; }
        [Description("Tier cache expiration in minutes")]
        public Ws.Core.Extensions.Data.Cache.Options.Duration EntryExpirationInMinutes { get; set; } = new();

    }
}