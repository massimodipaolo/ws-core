using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq;
using Microsoft.Extensions.Options;
using Ws.Core.Extensions.Data.Mongo;

namespace Ws.Core.Extensions.Data.Repository
{
    public class Mongo<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private readonly Mongo.Options _config;
        private readonly IMongoCollection<T> _collection;

        private IMongoCollection<T> getCollectionByConnection(string name)
        {
            var _db = _config.Connections?.FirstOrDefault(_ => _.Name == name);
            return new MongoClient(_db.ConnectionString).GetDatabase(_db.Database).GetCollection<T>(typeof(T).Name);
        }

        public Mongo(IOptions<Extensions.Data.Mongo.Options> config)
        {
            _config = config.Value;
            _collection = getCollectionByConnection("default");
        }

        IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

        public T Find(TKey Id)
        {
            return _collection.Find(_ => _.Id.Equals(Id)).FirstOrDefault();
        }

        public IQueryable<T> Query(FormattableString command)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            if (entity != null)
                _collection.InsertOne(entity);
        }

        public void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
                _collection.InsertMany(entities);
        }

        public void Update(T entity)
        {
            if (entity != null)
                _collection.ReplaceOneAsync(_ => _.Id.Equals(entity.Id), entity, new ReplaceOptions { IsUpsert = false });
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
                    _collection.Find(_ => !entities.Any(__ => __.Id.Equals(_.Id))).ForEachAsync(_ => Delete(_));
                }
                _collection.Find(_ => true).ForEachAsync(_ =>
                {
                    var entity = entities.FirstOrDefault(__ => __.Id.Equals(_.Id));
                    if (entity != null)
                        Update(entity);
                    else
                        Add(entity);
                });
            }
        }

        public void Delete(T entity)
        {
            if (entity != null)
                _collection.DeleteOne(_ => _.Id.Equals(entity.Id));
            //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Delete);
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
                _collection.DeleteMany(_ => entities.Any(__ => __.Id.Equals(_.Id)));
        }
    }
}
