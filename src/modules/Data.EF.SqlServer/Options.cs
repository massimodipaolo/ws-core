using System;
using System.Collections.Generic;
using System.Linq;
using Ws.Core.Extensions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data.EF.SqlServer
{
    public class Options : IOptions
    {
        public IEnumerable<Data.DbConnection> Connections { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
        public StoredProcedureConfig StoredProcedure { get; set; }
        public class StoredProcedureConfig
        {
            /// <summary>
            /// Stored procedure schema; default: dbo
            /// </summary>
            public string Schema { get; set; } = "dbo";
            public IEnumerable<MappingConfig> Mappings { get; set; }
            public class MappingConfig
            {
                public string NameSpace { get; set; }
                /// <summary>
                /// Entity type name
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// default: StoredProcedureConfig.Schema
                /// </summary>
                public string Schema { get; set; }
                /// <summary>
                /// Stored procedure name; default: Name
                /// </summary>
                public string StoredProcedure { get; set; }
                /// <summary>
                /// Call sp for this methods only; default: all methods
                /// </summary>
                public IEnumerable<MethodType> Methods { get; set; }
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

                public CommandTimeOutConfig CommandTimeOut { get; set; }
                /// <summary>
                /// Max execution time in seconds
                /// </summary>
                public class CommandTimeOutConfig
                {
                    public int Read { get; set; }
                    public int Write { get; set; }
                    public int Sync { get; set; }
                }
            }
        }
        public MergeConfig Merge { get; set; }
        public class MergeConfig : EFCore.BulkExtensions.BulkConfig
        {
            public new bool? UseTempDB { get; set; } = true;
            public new int? BulkCopyTimeout { get; set; } = 180;

            // Used for specifying custom properties, by which we want update to be done. If Identity column exisit and is not added in UpdateByProp it will be excluded automatically
            public new List<string> UpdateByProperties { get; set; }

            // Selected properties are excluded from being updated, can differ from PropertiesToExclude that can be used for Insert config only
            public new List<string> PropertiesToExcludeOnUpdate { get; set; }

            // By adding a column name to this list, will allow it to be inserted and updated but will not update the row if any of the others columns in that row did not change. For example, if importing data and want to keep an internal UpdateDate, add that column.
            public new List<string> PropertiesToExcludeOnCompare { get; set; }

            // When doing Insert/Update properties to affect can be explicitly selected by adding their names into PropertiesToInclude. If need to change more then half columns then PropertiesToExclude can be used. Setting both Lists are not allowed.
            public new List<string> PropertiesToExclude { get; set; }
            public new List<string> PropertiesToIncludeOnUpdate { get; set; }

            // By adding a column name to this list, will allow it to be inserted and updated but will not update the row if any of the these columns in that row did not change. For example, if importing data and want to keep an internal UpdateDate, add all columns except that one, or use PropertiesToExcludeOnCompare.
            public new List<string> PropertiesToIncludeOnCompare { get; set; }

            // When doing Insert/Update one or more properties can be exclude by adding their names into PropertiesToExclude. If need to change less then half column then PropertiesToInclude can be used. Setting both Lists are not allowed.
            public new List<string> PropertiesToInclude { get; set; }
        }
    }

}

namespace Ws.Core.Extensions.Data.EF.SqlServer.Extensions
{
    public static class OptionExtensions
    {
        public static bool? HasMethod(this Options.StoredProcedureConfig.MappingConfig mapping, Options.StoredProcedureConfig.MappingConfig.MethodType method)
            =>
            mapping == null || mapping.Methods == null || !mapping.Methods.Any() || mapping.Methods.Contains(method);
    }
}