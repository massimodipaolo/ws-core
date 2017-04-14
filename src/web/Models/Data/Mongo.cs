using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;

namespace web.Data
{
	public class Mongo<T>: IRepository<T> where T:IEntity
	{
		private Configuration.Settings.Db _db {get;set;}

		public string Connection { get { return $"mongodb://{_db.Host}:{_db.Port}";} }

		IMongoCollection<T> _collection;

		public Mongo(IOptions<Configuration.Settings> config)
		{			
            _db = config.Value.DbList?.FirstOrDefault(_ => _.Type == Configuration.Settings.Db.Types.Mongo);
            _collection = new MongoClient(Connection).GetDatabase(_db.Name).GetCollection<T>(typeof(T).Name);
		}

		public IEnumerable<T> List
		{
			get
			{
				return _list().Result;
			}
		}

		private async Task<IEnumerable<T>> _list() {
			return await _collection.Find(_ => true).ToListAsync();
		}

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
