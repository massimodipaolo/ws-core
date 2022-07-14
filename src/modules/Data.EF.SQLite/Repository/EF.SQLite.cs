using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository.EF
{
    public class SQLite<T, TKey> : EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        public SQLite(Ws.Core.Extensions.Data.EF.SQLite.DbContext context, IServiceProvider provider) : base(context, provider) { }
        public SQLite(Ws.Core.Extensions.Data.EF.SQLite.DbContext context, Ws.Core.Extensions.Data.EF.SQLite.DbConnectionFunctionWrapper funcWrapper, IServiceProvider provider)
        : base(context, funcWrapper.Func(typeof(T)), provider) { }
    }
}
