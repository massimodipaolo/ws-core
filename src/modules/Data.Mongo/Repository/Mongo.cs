using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq;
using Microsoft.Extensions.Options;
using core.Extensions.Data.Mongo;

namespace core.Extensions.Data.Repository
{
    public class Mongo<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private Mongo.Options _config;
        IMongoCollection<T> _collection;

        private IMongoCollection<T> getCollectionByConnection(string name)
        {
            var _db = _config.Connections?.FirstOrDefault(_ => _.Name == name);
            return new MongoClient(_db.ConnectionString).GetDatabase(_db.Database).GetCollection<T>(typeof(T).Name);
        }

        public Mongo(IOptions<core.Extensions.Data.Mongo.Options> config)
        {
            _config = config.Value;
            _collection = getCollectionByConnection("default");
        }

        IQueryable<T> IRepository<T, TKey>.List => _collection.AsQueryable();

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
            _collection.InsertOne(entity);
            //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Create);
        }

        public void Update(T entity)
        {
            _collection.ReplaceOneAsync(_ => _.Id.Equals(entity.Id), entity, new UpdateOptions { IsUpsert = true });
            //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Update);
        }

        public void Merge(IEnumerable<T> entities)
        {
            _collection.Find(_ => true).ForEachAsync(_ =>
            {
                var entity = entities.FirstOrDefault(__ => __.Id.Equals(_.Id));
                if (entity != null)
                    Update(entity);
                else
                    Add(entity);
            });
        }

        public void Delete(T entity)
        {
            _collection.DeleteOne(_ => _.Id.Equals(entity.Id));
            //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Delete);
        }
    }
}
