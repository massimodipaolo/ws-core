using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.Cache
{
    public interface ICache
    {
        object Get(string key);
        T Get<T>(string key);
        void Set(string key, object value);
        void Set(string key, object value, ICacheEntryOptions options);
        void Remove(string key);
    }
}
