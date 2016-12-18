using System;
using web.Data;
using Microsoft.AspNetCore.Mvc;
namespace web.Controllers
{
	public class EntityController<T> : Controller where T:IEntity
	{
		protected IRepository<T> _repository;

		public EntityController(IRepository<T> repository)
		{
			_repository = repository;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok(_repository.List);
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id)
		{
			return Ok(_repository.FindById(id));
		}

		[HttpPost]
		public void Post([FromBody]T entity)
		{
			_repository.Add(entity);
		}

		[HttpPut("{id}")]
		public void Put(string id, [FromBody]T entity)
		{
			_repository.Update(entity);
		}

		[HttpDelete]
		public void Delete([FromBody]T entity)
		{
			_repository.Delete(entity);
		}
	}
}
