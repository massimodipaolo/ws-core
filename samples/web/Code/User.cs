using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;

namespace web.Code
{
    public class User : Entity<Guid>
    {
        /// <summary>
        /// First Name + Last Name
        /// </summary>
        /// <example>Massimo Di Paolo</example>
        public string Name { get; set; }
        /// <example>Websolute</example>
        public string Company { get; set; }
        public LocaleText Bio { get; set; }
        public bool Active { get; set; } = true;
    }
}
