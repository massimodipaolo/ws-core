using System;
using System.Linq;
using core.Extensions.Data;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Data.Repository;
using core.Extensions.Cache.Repository;
using core.Extensions.Cache;

namespace core.Extensions.Api.Controllers
{
    public class EntityCachedController<T> : EntityController<T> where T : IEntity
    {        
        private readonly ICachedRepository<T> _cached;

        public EntityCachedController(IRepository<T> repository,ICachedRepository<T> cached) : base(repository)
        {
            _cached = cached;
        }

        [HttpGet]
        public override IActionResult Get()
        {
            return Ok(_cached.List);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(string id)
        {
            return Ok(_cached.Find(id));
        }

        [HttpPost]
        public override void Post([FromBody]T entity)
        {                        
            _cached.Add(entity);            
            base.Post(entity);
        }

        [HttpPut("{id}")]
        public override void Put(string id, [FromBody]T entity)
        {
            _cached.Update(entity);
            base.Put(id,entity);
        }

        [HttpDelete("{id}")]
        public override void Delete([FromBody]T entity)
        {
            _cached.Delete(entity);
            base.Delete(entity);
        }
    }
}
