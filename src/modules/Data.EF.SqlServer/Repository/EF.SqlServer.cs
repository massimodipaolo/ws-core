using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Extensions.Data.Repository.EF
{
    public class SqlServer<T, TKey>: EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static core.Extensions.Data.EF.SqlServer.Options.MergeConfig _mergeOptions { get; set; } = new core.Extensions.Data.EF.SqlServer.Extension()._options?.Merge ?? new core.Extensions.Data.EF.SqlServer.Options.MergeConfig();
        public SqlServer(AppDbContext context): base(context) {}

        public override void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    switch (operation)
                    {
                        case RepositoryMergeOperation.Upsert:
                            _context.BulkInsertOrUpdate<T>(entities.ToList(), _mergeOptions);
                            break;
                        case RepositoryMergeOperation.Sync:
                            _context.BulkInsertOrUpdateOrDelete<T>(entities.ToList(), _mergeOptions);
                            break;
                    }
                    transaction.Commit();
                }
            }
        }
    }
}
