using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Ws.Core.Extensions.Data.Repository.EF
{
    public class MySql<T, TKey> : EF<Ws.Core.Extensions.Data.EF.MySql.DbContext, T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public MySql(Ws.Core.Extensions.Data.EF.MySql.DbContext context, IServiceProvider provider) : base(context, provider) {}

        //public override string Info => _context.Set<EntityOfString>().FromSqlInterpolated($"select version() Id").AsEnumerable().FirstOrDefault().Id;
    }
}
