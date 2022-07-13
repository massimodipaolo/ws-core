using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache
{
    public interface IOptionEntryExpiration
    {
        EntryExpiration EntryExpirationInMinutes { get; set; }
    }
    public interface IOptionEntryExpiration<TCache>: IOptionEntryExpiration where TCache: ICache { }
    public class EntryExpiration
    {
        public EntryExpiration() { }

        [DefaultValue(10)]
        public double Fast { get; set; } = 10;
        [DefaultValue(60)]
        public double Medium { get; set; } = 60;
        [DefaultValue(240)]
        public double Slow { get; set; } = 240;
        [DefaultValue(1440)]
        public double Never { get; set; } = 1440;
    }
    public class Options : IOptions, IOptionEntryExpiration
    {
        [Description("Tier cache expiration in minutes")]
        public EntryExpiration EntryExpirationInMinutes { get; set; } = new EntryExpiration();
    }
    public class Options<TCache> : Options, IOptionEntryExpiration<TCache> where TCache : ICache { }

}