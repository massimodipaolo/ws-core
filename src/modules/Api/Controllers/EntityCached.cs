using System;
using core.Extensions.Data;
using core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;

namespace core.Extensions.Api.Controllers
{
    [Route("api/cache/[controller]")]
    public class EntityCachedController<T> : EntityController<T> where T : IEntity
    {
        protected ICacheRepository<T> _cachedRepository;

        public EntityCachedController(IRepository<T> repository, ICacheRepository<T> cached) : base(repository)
        {
            _cachedRepository = cached;
        }

        [HttpGet]
        public override IActionResult Get()
        {
            return Ok(_cachedRepository.List);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(Guid id)
        {
            return Ok(_cachedRepository.Find(id));
        }

        [HttpPost]
        public override void Post([FromBody]T entity)
        {
            _cachedRepository.Add(entity);
            //base.Post(entity);
        }

        [HttpPut("{id}")]
        public override void Put(string id, [FromBody]T entity)
        {
            _cachedRepository.Update(entity);
            //base.Put(id,entity);
        }

        [HttpDelete("{id}")]
        public override void Delete([FromBody]T entity)
        {
            _cachedRepository.Delete(entity);
            //base.Delete(entity);
        }
    }
}
