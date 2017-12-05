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
        public IEnumerable<Assembly> Assemblies { get; set; }        

        public class Assembly
        {
            public string Name { get; set; }
            public int Index { get; set; }
            //public core.Extensions.Base.IOptions Options { get; set; } //bind error
            
        }
    }
}
