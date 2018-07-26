using System;
using core.Extensions.Data;
using core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Api.Controllers;
using System.Collections.Generic;

namespace web.Controllers
{
    [Route("api/user")]
    public class UserController : EntityControllerWithMethods<User, Guid>
    {
        public UserController(IRepository<User, Guid> repository) : base(repository) { }

        [HttpPost]
        [Route("merge")]
        public IActionResult Merge([FromBody]IEnumerable<User> items)
        {
            _repository.Merge(items);
            return Ok();
        }
    }

    [Route("api/cache/user")]
    public class UserCacheController : EntityCachedController<User, Guid>
    {
        public UserCacheController(IRepository<User, Guid> repository, ICacheRepository<User, Guid> cachedRepository) : base(repository, cachedRepository) { }
    }


    public class User : Entity<Guid>
    {
        public string Name { get; set; }     
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
}