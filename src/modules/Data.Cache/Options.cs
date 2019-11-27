using System;
using System.Collections.Generic;
using System.Text;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Cache
{
    public class Options: IOptions
    {
            public Types Type { get; set; }
            public enum Types
            {
                Memory,
                Distributed
            }

            public static Duration EntryExpirationInMinutes { get; set; }
            public class Duration
            {
                public int Fast { get; set; } = 10;
                public int Medium { get; set; } = 60;
                public int Slow { get; set; } = 240;
                public int Never { get; set; } = 1440;
            }
    }
}