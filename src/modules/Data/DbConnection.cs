using System.Collections;
using System.ComponentModel;

namespace Ws.Core.Extensions.Data;

public interface IDbConnectionFunctionWrapper
{
    Func<Type, DbConnection> Func { get; }
}

public class DbConnection
{
    [Description("Connection alias")]
    [DefaultValue("default")]
    public string Name { get; set; } = "default";
    public string? ConnectionString { get; set; }
    public string? Database { get; set; }
    [Description("Criteria selectors (assembly/namespace/type) to map IEntity to a connection")]
    public ServiceResolverCriteria ServiceResolver { get; set; } = new();
    public class ServiceResolverCriteria
    {
        [Description("Criteria to include IEntity")]
        public ServiceResolverSelector Include { get; set; } = new();
        [Description("Criteria to exclude IEntity")]
        public ServiceResolverSelector Exclude { get; set; } = new();
    }
    public class ServiceResolverSelector
    {
        [Description("Type.Assembly, i.e. xCore")]
        public string[] Assembly { get; set; } = Array.Empty<string>();
        [Description("Type.Namespace, i.e. xCore.Models.Cms")]
        public string[] Namespace { get; set; } = Array.Empty<string>();
        [Description("Type.FullName, i.e. xCore.Models.Cms.User")]
        public string[] FullName { get; set; } = Array.Empty<string>();
    }
}

public class DbConnectionSelector
{
    private IEnumerable<Type> _entities { get; set; } = Array.Empty<Type>();
    private readonly DbConnection _dbConnection;
    public DbConnectionSelector(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public static Hashtable Collection(
        HashSet<DbConnectionSelector> connectionSelectorTable
        )
    {
        // repository resolver
        var entities = Ws.Core.Extensions.Base.Util.GetAllTypesOf(typeof(IEntity)) ?? Array.Empty<Type>();

        // normalize resolver rules
        var serviceResolver = connectionSelectorTable.Select(_ => _._dbConnection.ServiceResolver) ?? Array.Empty<DbConnection.ServiceResolverCriteria>();
        var serviceResolverWithAssemblyInclude = serviceResolver?.Where(_ => _.Include?.Assembly?.Any() == true) ?? Array.Empty<DbConnection.ServiceResolverCriteria>();
        serviceResolver?
            .Except(serviceResolverWithAssemblyInclude).AsParallel()
            .ForAll(_ => _.Exclude.Assembly = _.Exclude.Assembly.Concat(serviceResolverWithAssemblyInclude.SelectMany(_ => _.Include.Assembly).Distinct()).Distinct().ToArray());
        var serviceResolverWithNamespaceInclude = serviceResolver?.Where(_ => _.Include?.Namespace?.Any() == true) ?? Array.Empty<DbConnection.ServiceResolverCriteria>();
        serviceResolver?
            .Except(serviceResolverWithNamespaceInclude).AsParallel()
            .ForAll(_ => _.Exclude.Namespace = _.Exclude.Namespace.Concat(serviceResolverWithNamespaceInclude.SelectMany(_ => _.Include.Namespace).Distinct()).Distinct().ToArray());
        var serviceResolverWithFullNameInclude = serviceResolver?.Where(_ => _.Include?.FullName?.Any() == true) ?? Array.Empty<DbConnection.ServiceResolverCriteria>();
        serviceResolver?
            .Except(serviceResolverWithFullNameInclude).AsParallel()
            .ForAll(_ => _.Exclude.FullName = _.Exclude.FullName.Concat(serviceResolverWithFullNameInclude.SelectMany(_ => _.Include.FullName).Distinct()).Distinct().ToArray());

        foreach (var row in connectionSelectorTable)
        {
            var resolver = row._dbConnection.ServiceResolver;
            row._entities = entities.Where(_ =>
            (
            (!resolver.Include.Assembly.Any() || resolver.Include.Assembly?.Any(__ => __ == _.Assembly?.FullName) == true)
            &&
            (!resolver.Include.Namespace.Any() || resolver.Include.Namespace?.Any(__ => __ == _.Namespace) == true)
            &&
            (!resolver.Include.FullName.Any() || resolver.Include.FullName?.Any(__ => __ == _.FullName) == true)
            )
            &&
            !(
            resolver.Exclude.Assembly.Any(__ => __ == _.Assembly?.FullName)
            ||
            resolver.Exclude.Namespace.Any(__ => __ == _.Namespace)
            ||
            resolver.Exclude.FullName.Any(__ => __ == _.FullName)
            )
            );
        }

        Hashtable connectionSelectorHashtable = new();
        foreach (var entity in entities)
            connectionSelectorHashtable.Add(entity.FullName ?? entity.Name, connectionSelectorTable.FirstOrDefault(_ => _._entities.Contains(entity))?._dbConnection);

        return connectionSelectorHashtable;
    }

}
