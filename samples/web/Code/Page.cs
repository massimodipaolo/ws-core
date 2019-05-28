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
        public LocaleText Title { get; set; }
        public LocaleText Abstract { get; set; }
        public Meta Meta { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool? Enabled { get; set; } = true;
    }

    public class Meta
    {
        public LocaleText Title { get; set; }
        public LocaleText Description { get; set; }
        public LocaleText Keywords { get; set; }
    }
}
