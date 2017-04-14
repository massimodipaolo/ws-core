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
        private List<T> _collection = new List<T>();
        private string _path { get; set; }
        private Configuration.Db _config { get; set; }
        
        public FileSystem(IOptions<Configuration.Settings> config,IHostingEnvironment env)
        {
            _config = config.Value.Db?.FirstOrDefault(_ => _.Type==Configuration.Db.Types.FileSystem);
            _path = System.IO.Path.Combine(env.ContentRootPath, _config == null || string.IsNullOrEmpty(_config.Host) ? "Files/Entity" : _config.Host, $"{typeof(T).Name}.json");            

            using (var stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                var readed = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(readed))
                    _collection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(readed);
            }                
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

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id == entity.Id ? entity : _).ToList();
            Save();
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id == entity.Id);
            Save();
        }

        private void Save()
        {
            File.WriteAllText(_path,Newtonsoft.Json.JsonConvert.SerializeObject(_collection));
        }
    }
}
