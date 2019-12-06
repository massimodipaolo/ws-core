using Ws.Core.Extensions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data.EF
{
    public class Options: IOptions
    {
        /// <summary>
        /// Excludes the given entity type from the model. This method is typically used to remove types from the model that were added by convention., i.e. `MyNamespace.MyClass, MyAssembly`
        /// </summary>
        public string[] Ignore { get; set; }
        /// <summary>
        /// Serialize/deserialize type/interface, mapped on a text column
        /// </summary>
        public string[] JsonConvert { get; set; }
        public IEnumerable<MappingConfig> Mappings { get; set; }
        public IncludeNavigationPropertiesConfig IncludeNavigationProperties { get; set; }
        public class MappingConfig
        {
            public string NameSpace { get; set; }
            public string Name { get; set; }
            public string Table { get; set; }
            public string Schema { get; set; }
            public string IdColumnName { get; set; }
            public bool IdHasDefaultValue { get; set; } = true;
            public IEnumerable<PropertyConfig> Properties { get; set; }
            public class PropertyConfig
            {
                public string Name { get; set; }
                public string Column { get; set; }
                public bool Ignore { get; set; } = false;
                public bool? JsonConvert { get; set; }
            }
        }
        public class IncludeNavigationPropertiesConfig {
            public Operation List { get; set; }
            public Operation Find { get; set; } 
            public class Operation
            {
                /// <summary>
                /// Include the main navigation properties
                /// </summary>
                public bool Enabled { get; set; }
                /// <summary>
                /// List of typeof(T).FullName to exclude from then Enabled directive
                /// </summary>
                public IEnumerable<string> Except { get; set; }
                /// <summary>
                /// Set custom navigation paths. This setting overrides the Enabled/Except properties
                /// </summary>
                public IEnumerable<NavigationPaths> Explicit { get; set; }
                public class NavigationPaths
                {
                    /// <summary>
                    /// typeof(T).FullName
                    /// </summary>
                    public string Entity { get; set; }
                    /// <summary>
                    /// Array of indented navigation paths to include
                    /// i.e. for Order entity
                    /// [
                    ///     ["OrderDetail","Product","Supplier"],
                    ///     ["Customer"]
                    /// ]
                    /// </summary>
                    public IEnumerable<IEnumerable<string>> Paths { get; set; }
                }
            }
        }
    }
}
