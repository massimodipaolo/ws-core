using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;

namespace web.Code
{
    public class Language: Entity<int>
    {
        public string Mnemonic { get; set; }
        public bool Enabled { get; set; }
    }
}
