using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace core.Extensions.Data.Repository
{
    public class FileSystem<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private List<T> _collection = new List<T>();
        private string _path { get; set; }

        public FileSystem(IHostingEnvironment env, ILoggerFactory logger)
        {
            _path = System.IO.Path.Combine(env.ContentRootPath, "Files/Entity", $"{typeof(T).Name}.json");

            using (var stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                var readed = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(readed))
                    _collection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(readed);
                else
                    logger.CreateLogger("Data.Repository.Logger").LogWarning($"Path {_path} not found");
            }
        }


        IQueryable<T> IRepository<T, TKey>.List => _collection.AsQueryable();

        public T Find(TKey Id)
        {
            return _collection.FirstOrDefault<T>(_ => _.Id.Equals(Id));
        }

        public void Add(T entity)
        {
            _collection.Add(entity);
            Save();
            entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Create);
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id.Equals(entity.Id) ? entity : _).ToList();
            Save();
            entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Update);
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
            Save();
            entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Delete);
        }

        private void Save()
        {
            var jsonSetting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };
            File.WriteAllText(_path, Newtonsoft.Json.JsonConvert.SerializeObject(_collection, jsonSetting));
        }
    }
}
