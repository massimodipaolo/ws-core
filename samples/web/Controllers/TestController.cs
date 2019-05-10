using System;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Api.Controllers;
using System.Collections.Generic;

namespace web.Controllers
{
    /// <summary>
    /// Awesome user controller
    /// </summary>
    [Route("api/user")]
    public class TestController : EntityControllerWithMethods<User, Guid>
    {
        public TestController(IRepository<User, Guid> repository) : base(repository) { }

        /// <summary>
        /// Merge a list of users to the current IQueryable List
        /// </summary>
        /// <remarks>Merge Now!</remarks>
        /// <param name="items"></param>
        /// <returns><code>204</code></returns>                
        [HttpPost]
        [Route("merge")]
        public IActionResult Merge([FromBody]IEnumerable<User> items)
        {
            _repository.Merge(items);
            return Ok();
        }
    }

    /// <summary>
    /// Cached version of the Awesome user controller
    /// </summary>
    [Route("api/cache/user")]
    public class UserCacheController : EntityCachedController<User, Guid>
    {
        public UserCacheController(IRepository<User, Guid> repository, ICacheRepository<User, Guid> cachedRepository) : base(repository, cachedRepository) { }
    }

    /// <summary>
    /// App user entity
    /// </summary>
    public class User : Entity<Guid>
    {
        /// <summary>
        /// First Name + Last Name
        /// </summary>
        /// <example>Massimo Di Paolo</example>
        public string Name { get; set; }
        /// <example>Websolute</example>
        public string Company { get; set; }
        public string ToIgnore { get; set; }
        public bool Active { get; set; } = true;
    }

    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private ICache _cache;
        private Microsoft.Extensions.Configuration.IConfiguration _config;
        public ConfigController(Microsoft.Extensions.Configuration.IConfiguration config, ICache cache)
        {
            _config = config;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string key = "api:config";
            var result = _cache.Get(key);
            if (result == null)
            {
                result = _config;
                _cache.Set(key, result, CacheEntryOptions.Expiration.Fast);
            }
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _cache.Clear();
            return Ok();
        }

    }


    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private ICache _cache;
        public CacheController(ICache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_cache.Keys);
        }

        [HttpGet]
        [Route("clear")]
        public IActionResult Clear(string key)
        {
            _cache.Clear();
            return Ok(_cache.Keys);
        }

        [HttpGet]
        [Route("get/{key}")]
        public IActionResult GetKey(string key)
        {
            return Ok(_cache.Get<object>(key));
        }

        [HttpGet]
        [Route("set/{key}")]
        public IActionResult SetKey(string key)
        {
            _cache.Set(key, $"{key}-{DateTime.UtcNow}", CacheEntryOptions.Expiration.Fast);
            return Ok(_cache.Keys);
        }

        [HttpGet]
        [Route("remove/{key}")]
        public IActionResult RemoveKey(string key)
        {
            _cache.Remove(key);
            return Ok(_cache.Keys);
        }

    }
}