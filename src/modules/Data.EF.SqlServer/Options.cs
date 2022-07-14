using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.EF.SqlServer
{
    public class Options: IOptions
    {
        public IEnumerable<Data.DbConnection>? Connections { get; set; }
        [DefaultValue(ServiceLifetime.Scoped)]
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
        [Description("Use stored procedure instead of EF methods")]
        public StoredProcedureConfig? StoredProcedure { get; set; }
        public class StoredProcedureConfig
        {
            /// <summary>
            /// Stored procedure schema; default: dbo
            /// </summary>
            [DefaultValue("dbo")]
            public string Schema { get; set; } = "dbo";
            [Description("Map entity type to a set of stored procedure")]
            public IEnumerable<MappingConfig>? Mappings { get; set; }
            public class MappingConfig
            {
                public string? NameSpace { get; set; }
                /// <summary>
                /// Entity type name
                /// </summary>
                [Description("Entity type name => typeof(T).Name")]
                public string? Name { get; set; }
                /// <summary>
                /// default: StoredProcedureConfig.Schema
                /// </summary>
                [DefaultValue("main StoredProcedureConfig.Schema")]
                public string? Schema { get; set; }
                /// <summary>
                /// Stored procedure name; default: Name
                /// </summary>
                [Description("Stored procedure name. Will be trasformed in {schema}.entity_{name}_{method}")]
                public string? StoredProcedure { get; set; }

                /// <summary>
                /// Call sp for this methods only; default: all methods
                /// </summary>
                [Description("Call sp for one or more methods only: List,Find,Add,AddMany,Update,UpdateMany,Merge,Delete,DeleteMany; empty => all methods")]
                [EnumDataType(typeof(MethodType))]
                public string[]? Methods { get; set; }

                public enum MethodType
                {
                    List,
                    Find,
                    Add,
                    AddMany,
                    Update,
                    UpdateMany,
                    Merge,
                    Delete,
                    DeleteMany
                }

                /// <summary>
                /// Max execution time in seconds
                /// </summary>
                [Description("Max execution time in seconds")]
                public CommandTimeOutConfig CommandTimeOut { get; set; } = new();

                public class CommandTimeOutConfig
                {
                    [DefaultValue(60)]
                    public int Read { get; set; } = 60;
                    [DefaultValue(120)]
                    public int Write { get; set; } = 120;
                    [DefaultValue(180)]
                    public int Sync { get; set; } = 180;
                }
            }
        }
        [Description("EFCore.BulkExtensions.BulkConfig merge options")]
        public MergeConfig? Merge { get; set; }
        public class MergeConfig : EFCore.BulkExtensions.BulkConfig
        {
            public new bool? UseTempDB { get; set; } = true;
            public new int? BulkCopyTimeout { get; set; } = 180;
        }
    }

}

namespace Ws.Core.Extensions.Data.EF.SqlServer.Extensions
{
    public static class OptionExtensions
    {
        public static bool? HasMethod(this Options.StoredProcedureConfig.MappingConfig mapping, Options.StoredProcedureConfig.MappingConfig.MethodType method)
            => 
            mapping == null || mapping.Methods == null || !mapping.Methods.Any() || mapping.Methods.Any(_ => _.Equals(method.ToString()));
    }
}