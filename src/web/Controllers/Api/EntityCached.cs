using System;
using System.Linq;
using web.Data;
using web.Models;
using Microsoft.AspNetCore.Mvc;
namespace web.Controllers
{
    public class EntityCachedController<T> : EntityController<T> where T : IEntity
    {        
        protected static IRepository<T> _memory;

        public EntityCachedController(IRepository<T> repository) : base(repository)
        {
            if (_memory == null)                
                _memory = new Memory<T>(_repository.List.ToList());
        }

        [HttpGet]
        public override IActionResult Get()
        {
            return Ok(_memory.List);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(string id)
        {
            return Ok(_memory.Find(id));
        }

        [HttpPost]
        public override void Post([FromBody]T entity)
        {                        
            _memory.Add(entity);
            base.Post(entity);
        }

        [HttpPut("{id}")]
        public override void Put(string id, [FromBody]T entity)
        {
            _memory.Update(entity);
            base.Put(id,entity);
        }

        [HttpDelete("{id}")]
        public override void Delete([FromBody]T entity)
        {
            _memory.Delete(entity);
            base.Delete(entity);
        }
    }
}
