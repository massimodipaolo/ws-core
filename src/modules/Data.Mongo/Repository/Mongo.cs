using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq;
using core.Extensions.Data;

namespace core.Extensions.Data.Repository
{
	public class Mongo<T>: IRepository<T> where T:Entity
	{
		//private Configuration.Settings.Db _db {get;set;}

		IMongoCollection<T> _collection;

        /*
        public Mongo(IOptions<Configuration.Settings> config)
		{            
          //  _db = config.Value.DbList?.FirstOrDefault(_ => _.Type == Configuration.Settings.Db.Types.Mongo);
           // _collection = new MongoClient(_db.Connection).GetDatabase(_db.Name).GetCollection<T>(typeof(T).Name);                     
		} 
        */

        IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

        public void Add(T entity)
		{
            _collection.InsertOne(entity);
        }

		public void Delete(T entity)
		{
			_collection.DeleteOne(_ => _.Id == entity.Id); 
		}

		public void Update(T entity)
		{
            _collection.ReplaceOneAsync(_ => _.Id == entity.Id, entity, new UpdateOptions {IsUpsert = true});
        }

        public T Find(string Id)
		{
			return _collection.Find(_ => _.Id == Id).FirstOrDefault();
		}
	}
}
