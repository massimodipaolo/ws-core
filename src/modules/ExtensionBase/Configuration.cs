using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace core.Extensions.Base
{    
    public class Configuration
    {
        public IEnumerable<Assembly> Extensions { get; set; }

        public class Assembly
        {
            public string Name { get; set; }
            public int Index { get; set; }
        }
    }
}
