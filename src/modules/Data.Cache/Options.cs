using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache
{
    public class Options : IOptions
    {
        public Types Type { get; set; }
        public enum Types
        {
            Memory,
            Distributed
        }

        [Description("Tier cache expiration in minutes")]
        public Duration? EntryExpirationInMinutes { get; set; } = new Duration();
        public class Duration
        {
            [DefaultValue(10)]
            public int Fast { get; set; } = 10;
            [DefaultValue(60)]
            public int Medium { get; set; } = 60;
            [DefaultValue(240)] 
            public int Slow { get; set; } = 240;
            [DefaultValue(1440)] 
            public int Never { get; set; } = 1440;
        }
    }
}