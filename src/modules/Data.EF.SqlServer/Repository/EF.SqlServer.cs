using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Ws.Core.Extensions.Data.EF.SqlServer.Extensions;

namespace Ws.Core.Extensions.Data.Repository.EF;

public class SqlServer
{
    protected SqlServer() { }
    internal static Data.EF.SqlServer.Options options { get; set; } = new Data.EF.SqlServer.Extension().Options ?? new Data.EF.SqlServer.Options();
    internal static IEnumerable<Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig>? spMappings { get; set; }
}
public class SqlServer<T, TKey> : EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    private Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig? sp { get; set; }
    private const string spPrefix = "entity";

    public SqlServer(Ws.Core.Extensions.Data.EF.SqlServer.DbContext context, IServiceProvider provider) : base(context, provider)
    {
        Init();
    }

    public SqlServer(Ws.Core.Extensions.Data.EF.SqlServer.DbContext context, Ws.Core.Extensions.Data.EF.SqlServer.DbConnectionFunctionWrapper funcWrapper, IServiceProvider provider)
    : base(context, funcWrapper.Func(typeof(T)), provider)
    {
        Init();
    }

    private void Init()
    {
        if (SqlServer.spMappings == null)
        {
            var _spConfig = SqlServer.options?.StoredProcedure ?? new Data.EF.SqlServer.Options.StoredProcedureConfig();
            SqlServer.spMappings = _spConfig
                        .Mappings?
                        .Select(_ =>
                        {
                            _.Schema ??= _spConfig.Schema;
                            _.StoredProcedure ??= _.Name;
                            return _;
                        })
                        ??
                        new List<Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig>()
                        ;
        }

        sp = SqlServer.spMappings?
        .Where(_ =>
                _.Name == typeof(T).Name
                && (string.IsNullOrEmpty(_.NameSpace) || _.NameSpace == typeof(T).Namespace)
            )?
            .FirstOrDefault();
    }

    #region Override repo
    public override IQueryable<T> List => sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.List) ?? false ? list : base.List;
    private IQueryable<T> list
    {
        get
        {
            var json = ExecuteScalar(default).Result;
            if (string.IsNullOrEmpty(json)) return Array.Empty<T>().AsQueryable();

            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(json)?.AsQueryable() ?? Array.Empty<T>().AsQueryable();
        }
    }
    public override T? Find(TKey? Id) => sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Find) ?? false ? find(Id) : base.Find(Id);
    private T? find(TKey? Id)
    {
        var json = ExecuteScalar(Id).Result;
        if (string.IsNullOrEmpty(json)) return null;

        return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(json)?.FirstOrDefault();
    }
    public override void Add(T entity)
    {
        if (entity != null)
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Add) ?? false)
                ExecuteCrudCommand(entity, "insert");
            else
                base.Add(entity);
    }

    public override void AddMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.AddMany) ?? false)
                ExecuteCrudCommand(entities, "insertmany");
            else
                base.AddMany(entities);
    }

    public override void Update(T entity)
    {
        if (entity != null)
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Update) ?? false)
                ExecuteCrudCommand(entity, "update");
            else
                base.Update(entity);
    }

    public override void UpdateMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.UpdateMany) ?? false)
                ExecuteCrudCommand(entities, "updatemany");
            else
                base.UpdateMany(entities);
    }

    public override void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
    {
        if (entities != null && entities.Any())
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Merge) ?? false)
                ExecuteMergeCommand(entities, operation);
            else
                merge(entities, operation);
    }

    private void merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
    {
        if (entities != null)
        {
            using var transaction = _context.Database.BeginTransaction();
            switch (operation)
            {
                case RepositoryMergeOperation.Upsert:
                    _context.BulkInsertOrUpdate<T>(entities.ToList(), SqlServer.options.Merge);
                    break;
                case RepositoryMergeOperation.Sync:
                    _context.BulkInsertOrUpdateOrDelete<T>(entities.ToList(), SqlServer.options.Merge);
                    break;
            }
            transaction.Commit();
        }
    }

    public override void Delete(T entity)
    {
        if (entity != null)
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Delete) ?? false)
                ExecuteCrudCommand(entity, "delete");
            else
                base.Delete(entity);
    }

    public override void DeleteMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
            if (sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.DeleteMany) ?? false)
                ExecuteCrudCommand(entities, "deletemany");
            else
                base.DeleteMany(entities);
    }

    #endregion

    #region Stored Procedure

    private void ExecuteCrudCommand(T entity, string action)
    {
        if (entity != null)
            ExecuteCrudCommand((object)entity, action);
    }

    private void ExecuteCrudCommand(IEnumerable<T> entities, string action)
    {
        if (entities?.Any() == true)
            ExecuteCrudCommand((object)entities, action);
    }

    private void ExecuteCrudCommand(object obj, string action)
    {
        if (sp != null)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            db<Data.EF.SqlServer.DbContext>()?.SetCommandTimeout(sp.CommandTimeOut.Write);
            var cmd = $"exec [{sp.Schema}].[{spPrefix}_{sp.StoredProcedure}_{action}] {{0}}";
            var query = System.Runtime.CompilerServices.FormattableStringFactory.Create(cmd, data);
            db<Data.EF.SqlServer.DbContext>()?.ExecuteSqlInterpolated(query);
        }
    }



    private void ExecuteMergeCommand(IEnumerable<T> entities, RepositoryMergeOperation operation)
    {
        if (entities != null)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(entities);
            ExecuteMergeCommand(data, operation);
        }
    }

    private void ExecuteMergeCommand(string data, RepositoryMergeOperation operation)
    {
        if (sp != null)
        {
            db<Data.EF.SqlServer.DbContext>()?.SetCommandTimeout(sp.CommandTimeOut.Sync);
            var cmd = $"exec [{sp.Schema}].[{spPrefix}_{sp.StoredProcedure}_merge] {{0}},{{1}}";
            var query = System.Runtime.CompilerServices.FormattableStringFactory.Create(cmd, data, operation);
            db<Data.EF.SqlServer.DbContext>()?.ExecuteSqlInterpolated(query);
        }
    }

    private async Task<string> ExecuteScalar(TKey? Id)
    {
        var result = new System.Text.StringBuilder();

        if (sp != null)
        {
            var cmd = db<Data.EF.SqlServer.DbContext>()?.GetDbConnection().CreateCommand();
            if (cmd != null)
            {
                cmd.CommandText = $"[{sp.Schema}].{spPrefix}_{sp.StoredProcedure}_select";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = sp.CommandTimeOut.Read;

                var param = cmd.CreateParameter();
                param.ParameterName = "@id";
                if (Id != null && !Id.Equals(default))
                    param.Value = Id;
                else
                    param.Value = DBNull.Value;
                cmd.Parameters.Add(param);

                using (cmd)
                {
                    try
                    {
                        if (cmd.Connection?.State != System.Data.ConnectionState.Open)
                            cmd.Connection?.Open();
                        using var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false);
                        while (await reader.ReadAsync().ConfigureAwait(false))
                            result.Append(reader.GetString(0));
                    }
                    finally
                    {
                        cmd.Connection?.Close();
                    }
                }
            }
        }
        return result.ToString();
    }

    #endregion


}
