﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository.EF
{
    public class SQLite<T, TKey> : EF<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public SQLite(AppDbContext context, IServiceProvider provider) : base(context, provider) {}
        //public override string Info => _context.Set<EntityOfString>().FromSqlInterpolated($"select sqlite_version() Id").AsEnumerable().FirstOrDefault().Id;
    }
}