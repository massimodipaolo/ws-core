using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.EF;

public class Options: IOptions
{
    /// <summary>
    /// Excludes the given entity type from the model. This method is typically used to remove types from the model that were added by convention., i.e. `MyNamespace.MyClass, MyAssembly`
    /// </summary>
    [Description("Excludes the given entity type from the model. This method is typically used to remove types from the model that were added by convention., i.e. `MyNamespace.MyClass, MyAssembly`")]
    public string[]? Ignore { get; set; }
    /// <summary>
    /// Serialize/deserialize type/interface, mapped on a text column
    /// </summary>
    [Description("Serialize/deserialize type/interface, mapped on a text column")]
    public string[]? JsonConvert { get; set; }
    public IEnumerable<MappingConfig>? Mappings { get; set; }
    public IncludeNavigationPropertiesConfig? IncludeNavigationProperties { get; set; }
    public class MappingConfig
    {
        public string? NameSpace { get; set; }
        public string? Name { get; set; }
        public string? Table { get; set; }
        public string? Schema { get; set; }
        public string? IdColumnName { get; set; }
        public bool IdHasDefaultValue { get; set; } = true;
        public IEnumerable<PropertyConfig>? Properties { get; set; }
        public class PropertyConfig
        {
            public string? Name { get; set; }
            public string? Column { get; set; }
            public bool Ignore { get; set; } = false;
            public bool? JsonConvert { get; set; }
            public string? HasConversion { get; set; }
            
            public static readonly Type[] ColumnClrTypeConversions =
            {
                typeof(string),
                typeof(int),
                typeof(long),
                typeof(bool),
                typeof(char),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid),
                typeof(byte[])
          };
        }
    }
    public class IncludeNavigationPropertiesConfig {
        public Operation? List { get; set; }
        public Operation? Find { get; set; } 
        public class Operation
        {
            /// <summary>
            /// Include the main navigation properties
            /// </summary>
            [Description("Include the main navigation properties")]
            [DefaultValue(false)]
            public bool Enable { get; set; } = false;
            /// <summary>
            /// List of typeof(T).FullName to exclude from then Enabled directive
            /// </summary>
            [Description("List of typeof(T).FullName to exclude from then Enabled directive")]
            public string[]? Except { get; set; } = Array.Empty<string>();
            /// <summary>
            /// Set custom navigation paths. This setting overrides the Enabled/Except properties
            /// </summary>
            [Description("Set custom navigation paths. This setting overrides the Enabled/Except properties")]
            public IEnumerable<NavigationPaths>? Explicit { get; set; }
            public class NavigationPaths
            {
                /// <summary>
                /// typeof(T).FullName
                /// </summary>
                [Description("typeof(T).FullName")]
                public string? Entity { get; set; }
                /// <summary>
                /// Array of indented navigation paths to include
                /// i.e. for Order entity
                /// [
                ///     ["OrderDetail","Product","Supplier"],
                ///     ["Customer"]
                /// ]
                /// </summary>
                [Description("Array of indented navigation paths to include" +
                    "i.e. for Order entity" +
                    "[" +
                    "   [\"OrderDetail\",\"Product\",\"Supplier\"]," +
                    "   [\"Customer\"]" +
                    "]"
                    )]
                public IEnumerable<IEnumerable<string>>? Paths { get; set; }
            }
        }
    }
}
