using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using core.Extensions.Data.EF.SqlServer.Extensions;

namespace core.Extensions.Data.Repository.EF
{
    public class SqlServer<T, TKey> : EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static Data.EF.SqlServer.Options _options { get; set; } = new Data.EF.SqlServer.Extension()._options ?? new Data.EF.SqlServer.Options();
        private static IEnumerable<Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig> _spMappings { get; set; }
        private Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig _sp { get; set; }
        private IServiceProvider _provider { get; set; }
        private const string _spPrefix = "entity";
        public SqlServer(AppDbContext context, IServiceProvider provider) : base(context)
        {
            if (_spMappings == null)
            {
                var _spConfig = (_options?.StoredProcedure ?? new core.Extensions.Data.EF.SqlServer.Options.StoredProcedureConfig());
                _spMappings = _spConfig
                            .Mappings?
                            .Select(_ =>
                            {
                                _.Schema = _.Schema ?? _spConfig.Schema;
                                _.StoredProcedure = _.StoredProcedure ?? _.Name;
                                return _;
                            })
                            ??
                            new List<core.Extensions.Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig>()
                            ;
            }

            _sp = _spMappings
            .Where(_ =>
                    _.Name == typeof(T).Name
                    && (string.IsNullOrEmpty(_.NameSpace) || _.NameSpace == typeof(T).Namespace)
                )?
                .FirstOrDefault();

            _provider = provider;
        }

        #region Override repo
        public override IQueryable<T> List => _sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.List) ?? false ? _List : base.List;
        private IQueryable<T> _List
        {
            get
            {
                var json = ExecuteScalar(default(TKey)).Result;
                if (string.IsNullOrEmpty(json)) return null;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(json).AsQueryable();
            }
        }
        public override T Find(TKey Id) => _sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Find) ?? false ? _Find(Id) : base.Find(Id);
        private T _Find(TKey Id)
        {
            var json = ExecuteScalar(Id).Result;
            if (string.IsNullOrEmpty(json)) return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(json)?.FirstOrDefault();
        }
        public override void Add(T entity)
        {
            if (_sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Add) ?? false)
                ExecuteCrudCommand(entity, "insert");
            else
                base.Add(entity);
        }
        public override void Update(T entity)
        {
            if (_sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Update) ?? false)
                ExecuteCrudCommand(entity, "update");
            else
                base.Update(entity);
        }

        public override void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (_sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Merge) ?? false)                            
                ExecuteMergeCommand(entities, operation);            
            else
                _Merge(entities, operation);
        }

        private void _Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null)
            {
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        switch (operation)
                        {
                            case RepositoryMergeOperation.Upsert:
                                _context.BulkInsertOrUpdate<T>(entities.ToList(), _options.Merge);
                                break;
                            case RepositoryMergeOperation.Sync:
                                _context.BulkInsertOrUpdateOrDelete<T>(entities.ToList(), _options.Merge);
                                break;
                        }
                        transaction.Commit();
                    }
                }
            }
        }

        public override void Delete(T entity)
        {
            if (_sp?.HasMethod(Data.EF.SqlServer.Options.StoredProcedureConfig.MappingConfig.MethodType.Delete) ?? false)
                ExecuteCrudCommand(entity, "delete");
            else
                base.Delete(entity);
        }
        #endregion

        #region Stored Procedure

        private void ExecuteCrudCommand(T entity, string action)
        {
            if (entity != null)
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
                ExecuteCrudCommand(action, data);
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

        private void ExecuteCrudCommand(string action, string data)
        {
            var param = "@data";
            _db().ExecuteSqlCommand(
                $"exec {_sp.Schema}.{_spPrefix}_{_sp.StoredProcedure}_{action} {param}",
                new System.Data.SqlClient.SqlParameter(param, data)
                );
        }

        private void ExecuteMergeCommand(string data, RepositoryMergeOperation operation)
        {
            var p1 = "@data";
            var p2 = "@operation";
            _db().ExecuteSqlCommand(
                $"exec {_sp.Schema}.{_spPrefix}_{_sp.StoredProcedure}_merge {p1},{p2}",
                new System.Data.SqlClient.SqlParameter(p1, data),
                new System.Data.SqlClient.SqlParameter(p2, operation.ToString())
                );
        }


        private async Task<string> ExecuteScalar(TKey Id)
        {
            var result = new System.Text.StringBuilder();

            var cmd = _db().GetDbConnection().CreateCommand();
            cmd.CommandText = $"{_sp.Schema}.{_spPrefix}_{_sp.StoredProcedure}_select";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandTimeout = 180;

            var param = cmd.CreateParameter();
            param.ParameterName = "@id";
            if (Id != null && !Id.Equals(default(TKey)))
                param.Value = Id;
            else
                param.Value = DBNull.Value;
            cmd.Parameters.Add(param);

            using (cmd)
            {
                try
                {
                    if (cmd.Connection.State != System.Data.ConnectionState.Open)
                        cmd.Connection.Open();
                }
                catch { }

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.Default).ConfigureAwait(false))
                        while (await reader.ReadAsync().ConfigureAwait(false))
                            result.Append(reader.GetString(0));
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
            return result.ToString();
        }


        private Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade _db()
        {
            Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade db = null;
            try
            {
                db = _context.Database;
            }
            catch  // context disposed
            {
                db = _provider.GetService<AppDbContext>()?.Database;
            }
            return db;
        }

        #endregion


    }
}
