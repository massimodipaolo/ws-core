using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace core.Extensions.Base
{    
    public class Configuration
    {
        public string Path { get; set; }
        public bool EnableShutDownOnChange { get; set; } = false;
        public IDictionary<string,Assembly> Assemblies { get; set; }

        public class Assembly
        {
            public Assembly() { }
            public string Name { get; set; }
            public int Priority { get; set; } = 0;
        }

    }
}
