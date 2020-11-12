using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Data;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class EntityController<T, TKey> : ControllerBase where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected IRepository<T, TKey> _repository;

        public EntityController(IRepository<T, TKey> repository)
        {
            _repository = repository;
        }
    }

    [Route("api/[controller]")]
    public class EntityControllerWithMethods<T, TKey> : EntityController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public EntityControllerWithMethods(IRepository<T, TKey> repository): base(repository) {}

        /// <summary>
        /// Retrieves a IQueryable<typeparamref name="T"/>
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Product created</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        [HttpGet]        
        public virtual IActionResult Get()
        {
            return Ok(_repository.List);
        }

        [HttpGet("{id}")]            
        public virtual IActionResult GetById(TKey id)
        {
            return Ok(_repository.Find(id));
        }

        [HttpPost]
        public virtual void Post([FromBody]T entity)
        {
            _repository.Add(entity);
        }

        [Route("many")]
        [HttpPost]
        public virtual void PostMany([FromBody]IEnumerable<T> entities)
        {
            _repository.AddMany(entities);
        }

        [HttpPut("{id}")]
        public virtual void Put(TKey id, [FromBody]T entity)
        {
            _repository.Update(entity);
        }

        [HttpPut]
        public virtual void PutMany([FromBody]IEnumerable<T> entities)
        {
            _repository.UpdateMany(entities);
        }

        [Route("merge/{operation}")]
        [HttpPost]
        public virtual void Merge(RepositoryMergeOperation operation, [FromBody]IEnumerable<T> entities)
        {
            _repository.Merge(entities, operation);
        }

        [HttpDelete("{id}")]
        public virtual void Delete([FromBody]T entity)
        {
            _repository.Delete(entity);
        }

        [HttpDelete]
        public virtual void DeleteMany([FromBody]IEnumerable<T> entities)
        {
            _repository.DeleteMany(entities);
        }
    }
}
