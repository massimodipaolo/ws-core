using Microsoft.Extensions.Caching.SqlServer;
using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache.SqlServer
{
    public class Options : IOptions, IOptionEntryExpiration
    {
        [Description("SqlServer cache client")]
        [DefaultValue("ConnString: local Trusted; Tbl: dbo.Entry")]
        public SqlServerCacheOptions? Client { get; set; }
        [Description("Tier cache expiration in minutes")]
        public Ws.Core.Extensions.Data.Cache.EntryExpiration EntryExpirationInMinutes { get; set; } = new();

    }
}