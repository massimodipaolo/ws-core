using System;

namespace Ws.Core.Extensions.Data.Repository.EF;

public class MySql<T, TKey> : EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public MySql(Ws.Core.Extensions.Data.EF.MySql.DbContext context, IServiceProvider provider) : base(context, provider) { }
    public MySql(Ws.Core.Extensions.Data.EF.MySql.DbContext context, Ws.Core.Extensions.Data.EF.MySql.DbConnectionFunctionWrapper funcWrapper, IServiceProvider provider)
    : base(context, funcWrapper.Func(typeof(T)), provider) { }

}
