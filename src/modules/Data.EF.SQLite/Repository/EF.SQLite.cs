using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository.EF
{
    public class SQLite<T, TKey> : EF<Ws.Core.Extensions.Data.EF.DbContext, T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public SQLite(Ws.Core.Extensions.Data.EF.SQLite.DbContext context, IServiceProvider provider) : base(context, provider) { }
        public SQLite(Ws.Core.Extensions.Data.EF.SQLite.DbContextFunctionWrapper funcWrapper, IServiceProvider provider)
        : base(funcWrapper.Func(typeof(T)), provider) { }
    }
}
