using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache
{
    public interface ICache
    {
        IEnumerable<string> Keys { get; }
        object Get(string key);
        T Get<T>(string key);
        void Set(string key, object value);
        void Set(string key, object value, ICacheEntryOptions options);
        void Remove(string key);
        void Clear();
    }
}
