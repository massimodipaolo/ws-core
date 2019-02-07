using core.Extensions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.EF
{
    public class Options: IOptions
    {
        public string[] Ignore { get; set; }
        public IEnumerable<MappingConfig> Mappings { get; set; }
        public class MappingConfig
        {
            public string NameSpace { get; set; }
            public string Name { get; set; }
            public string Table { get; set; }
            public string Schema { get; set; }
            public string IdColumnName { get; set; }
            public IEnumerable<PropertyConfig> Properties { get; set; }
            public class PropertyConfig
            {
                public string Name { get; set; }
                public string Column { get; set; }
                public bool Ignore { get; set; } = false;
            }
        }
    }
}
