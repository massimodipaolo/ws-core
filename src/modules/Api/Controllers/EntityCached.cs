using System;
using core.Extensions.Data;
using core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;

namespace core.Extensions.Api.Controllers
{
    [Route("api/cache/[controller]")]
    public class EntityCachedController<T, TKey> : EntityController<T, TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected ICacheRepository<T, TKey> _cachedRepository;

        public EntityCachedController(IRepository<T, TKey> repository, ICacheRepository<T, TKey> cached) : base(repository)
        {
            _cachedRepository = cached;
        }

        [HttpGet]
        public override IActionResult Get()
        {
            return Ok(_cachedRepository.List);
        }

        [HttpGet("{id}")]
        public override IActionResult Get(TKey id)
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
        public override void Put(TKey id, [FromBody]T entity)
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
