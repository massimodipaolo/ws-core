using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace web.Data
{
    public class FileSystem<T> : IRepository<T> where T : IEntity
    {
        ICollection<T> _collection;
        private string _path { get; set; }
        private Db _config { get; set; }
        
        public FileSystem(IOptions<Configuration> config,IHostingEnvironment env)
        {
            _config = config.Value.Db[0];
            _path = System.IO.Path.Combine(env.ContentRootPath,string.IsNullOrEmpty(_config.Host) ? "Files/Entity" : _config.Host, $"{typeof(T).Name}.json");
            if (File.Exists(_path))
                _collection = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(File.ReadAllText(_path)).ToList();
            else
                File.Create(_path);
        }
        public IEnumerable<T> List
        {
            get
            {
                return _collection;
            }
        }

        public T Find(string Id)
        {
            return _collection.Where(_ => _.Id == Id).FirstOrDefault();
        }
        public void Add(T entity)
        {
            _collection.Add(entity);
            Save();
        }

        public void Delete(T entity)
        {
            _collection.Remove(entity);
            Save();
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id == entity.Id ? entity : _).ToList();
            Save();
        }

        private void Save()
        {
            File.WriteAllText(_path,Newtonsoft.Json.JsonConvert.SerializeObject(_collection));
        }
    }
}
