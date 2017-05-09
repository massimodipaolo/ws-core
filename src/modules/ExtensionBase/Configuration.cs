using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace core.Extension.Configuration
{    
    public class Options
    {
        public IEnumerable<Extension> Extensions { get; set; }

        public class Extension
        {
            public string Name { get; set; }
            public int Index { get; set; }
        }
    }
}
