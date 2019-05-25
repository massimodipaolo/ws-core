using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;

namespace web.Code
{
    public class Page: Entity<int>
    {
        public string Mnemonic { get; set; }
        public LocaleTexts Title { get; set; }
        public LocaleTexts Abstract { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool Enabled { get; set; }
    }
}
