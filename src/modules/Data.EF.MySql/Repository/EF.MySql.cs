using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Ws.Core.Extensions.Data.Repository.EF
{
    public class MySql<T, TKey> : EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public MySql(AppDbContext context, IServiceProvider provider) : base(context, provider) {}

        //public override string Info => _context.Set<EntityOfString>().FromSqlInterpolated($"select version() Id").AsEnumerable().FirstOrDefault().Id;
    }
}
