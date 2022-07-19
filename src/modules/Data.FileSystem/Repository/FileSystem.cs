using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Ws.Core.Extensions.Data.Repository;

public class FileSystem: BaseRepository
{
    protected static Data.FileSystem.Options _options { get; set; } = new Data.FileSystem.Extension().Options ?? new Data.FileSystem.Options();
}
public class FileSystem<T, TKey> : FileSystem, IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    private List<T> _collection = new();
    private string? _path { get; set; } 

    public FileSystem(IWebHostEnvironment env, ILoggerFactory logger)
    {
        Type type = typeof(T);
        var names = new string[] { type.FullName ?? type.Name, type.Name }.Distinct();
        foreach (string name in names)
        {
            var _search = System.IO.Path.Combine(env.ContentRootPath, _options.Folder, $"{name}.json");
            if (File.Exists(_search))
            {
                _path = _search;
                using var stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                var readed = reader.ReadToEnd();
                var jsonSetting = _options?.Serialization?.ToJsonSerializerSettings();
                if (!string.IsNullOrEmpty(readed))
                    _collection = System.Text.Json.JsonSerializer.Deserialize<List<T>>(readed, jsonSetting) ?? _collection;
                break;
            }
        }
        if (_path == null)
            logger
                .CreateLogger($"Data.Repository.{nameof(FileSystem)}")
                .LogWarning("File '{files}' not found in '{folder}'", string.Join(",", names), System.IO.Path.Combine(env.ContentRootPath, _options?.Folder ?? ""));
    }


    IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

    public T? Find(TKey? Id)
    {
        return _collection.FirstOrDefault<T>(_ => _.Id?.Equals(Id) == true);
    }

    public IQueryable<T> Query(FormattableString command)
    {
        throw new NotImplementedException();
    }

    public void Add(T entity)
    {
        if (entity != null)
        {
            _collection.Add(entity);
            Save();
        }
    }

    public void AddMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
        {
            _collection.AddRange(entities);
            Save();
        }
    }

    public void Update(T entity)
    {
        if (entity != null)
        {
            var item = Find(entity.Id);
            if (item != null)
                _collection[_collection.IndexOf(item)] = entity;
            Save();       
        }
    }
    public void UpdateMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
        {
            var joined = _collection.Join(entities, o => o.Id, i => i.Id, (o, i) => (o, i));
            for (int i = joined.Count() - 1; i > -1; --i)
            {
                var _ = joined.ElementAt(i);
                _collection[_collection.IndexOf(_.o)] = _.i;
            }                    
            
            Save();
        }
    }

    public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
    {
        if (entities != null && entities.Any())
        {
            switch (operation)
            {
                case RepositoryMergeOperation.Upsert:
                    _collection = entities.Union(_collection, new EntityComparer<T, TKey>()).ToList();
                    break;
                case RepositoryMergeOperation.Sync:
                    _collection = entities.ToList();
                    break;
            }
            Save();
        }
    }

    public void Delete(T entity)
    {
        if (entity != null)
        {
            _collection.RemoveAll(_ => _.Id?.Equals(entity.Id) == true);
            Save();
        }
    }

    public void DeleteMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
        {
            _collection.RemoveAll(_ => entities.Any(__ => __.Id?.Equals(_.Id) == true));
            Save();
        }
    }

    private void Save()
    {
        if (_path != null)
        {
            var jsonSetting = _options?.Serialization?.ToJsonSerializerSettings();
            File.WriteAllText(_path, System.Text.Json.JsonSerializer.Serialize(_collection, jsonSetting));
        }
    }
}
