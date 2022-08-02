using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ws.Core.Extensions.Data.Repository;

public class Mongo<T, TKey> : BaseRepository, IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    private readonly Mongo.Options _config;
    private readonly IMongoCollection<T>? _collection;

    private IMongoCollection<T>? getCollectionByConnection(string name)
    {
        var _db = _config.Connections?.FirstOrDefault(_ => _.Name == name);
        if (null == _db) return null;
        IMongoDatabase db = new MongoClient(_db.ConnectionString).GetDatabase(_db.Database);
        var collection = db.ListCollectionNames().ToEnumerable()
            .FirstOrDefault(_ => new string[] { typeof(T)?.FullName ?? typeof(T).Name, typeof(T).Name }.Contains(_, System.StringComparer.OrdinalIgnoreCase));
        if (collection != null)
            return db.GetCollection<T>(collection, new MongoCollectionSettings() { AssignIdOnInsert = false });
        return null;
    }

    public Mongo(IOptions<Extensions.Data.Mongo.Options> config)
    {
        _config = config.Value;
        _collection = getCollectionByConnection("default");
    }

    IQueryable<T> IRepository<T>.List => _collection?.AsQueryable() ?? Array.Empty<T>().AsQueryable();

    public T? Find(TKey? Id)
    {
        return _collection?.Find<T>(_ => (_.Id is TKey) && _.Id.Equals(Id))?.FirstOrDefault() ?? default;
    }

    public IQueryable<T> Query(FormattableString command)
    {
        throw new NotImplementedException();
    }

    public void Add(T entity)
    {
        if (entity != null)
            _collection?.InsertOne(entity);
    }

    public void AddMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
            _collection?.InsertMany(entities);
    }

    public void Update(T entity)
    {
        if (entity != null)
            _collection?.ReplaceOne<T>(_ => (_.Id is TKey) && _.Id.Equals(entity.Id), entity, new ReplaceOptions { IsUpsert = false });
    }

    public void UpdateMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
            foreach (var entity in entities)
                Update(entity);
    }

    public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
    {
        if (entities != null && entities.Any())
        {
            if (operation == RepositoryMergeOperation.Sync)
            {
                _collection?.DeleteMany(_ => true);
                _collection?.InsertMany(entities);
            }
            else
                foreach (var entity in entities)
                {
                    if (Find(entity.Id) != null)
                        Update(entity);
                    else
                        Add(entity);
                }
        }
    }

    public void Delete(T entity)
    {
        if (entity != null)
            _collection?.DeleteOne<T>(_ => (_.Id is TKey) &&  _.Id.Equals(entity.Id));
    }

    public void DeleteMany(IEnumerable<T> entities)
    {
        if (entities != null && entities.Any())
            foreach (var entity in entities)
                Delete(entity);
    }
}
