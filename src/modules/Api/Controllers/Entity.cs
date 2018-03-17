using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Data;

namespace core.Extensions.Api.Controllers
{
    [Route("api/[controller]")]
    public class EntityController<T, TKey> : ControllerBase where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected IRepository<T, TKey> _repository;

        public EntityController(IRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            return Ok(_repository.List);
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(TKey id)
        {
            return Ok(_repository.Find(id));
        }

        [HttpPost]
        public virtual void Post([FromBody]T entity)
        {
            _repository.Add(entity);
        }

        [HttpPut("{id}")]
        public virtual void Put(TKey id, [FromBody]T entity)
        {

            _repository.Update(entity);
        }

        [HttpDelete("{id}")]
        public virtual void Delete([FromBody]T entity)
        {
            _repository.Delete(entity);
        }
    }
}
