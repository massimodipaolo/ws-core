using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace web.Data
{
	public class Mongo<T>: IRepository<T> where T:IEntity
	{
		private Db _config {get;set;}

		public string Connection { get { return $"mongodb://{_config.Host}:{_config.Port}";} }

		IMongoCollection<T> _collection;

		public Mongo(IOptions<Configuration> config)
		{
			_config = config.Value.Db;
			_collection = new MongoClient(Connection).GetDatabase(_config.Name).GetCollection<T>(typeof(T).Name);
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
			throw new NotImplementedException();
		}

		public void Delete(T entity)
		{
			_collection.DeleteOne(_ => _.Id == entity.Id); 
		}

		public void Update(T entity)
		{
			throw new NotImplementedException();
		}

		public T FindById(string Id)
		{
			return _collection.Find(_ => _.Id == Id).FirstOrDefault();
		}
	}
}
