using System;
using System.Collections.Generic;
namespace web.Data
{
	public interface IDb<T> where T:IEntity
	{
#warning TODO: implement, i.e. IRelationalDb, IDocumentDb...
		string Connection { get;}
		IRepository<T> Repository { get; set; }
	}

	public interface IRepository<T> where T:IEntity
	{
		IEnumerable<T> List { get;}
		void Add(T entity);
		void Delete(T entity);
		void Update(T entity);
		T FindById(string Id);
	}

}
