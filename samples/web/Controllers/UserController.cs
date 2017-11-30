using System;
using core.Extensions.Data;
using core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Api.Controllers;

namespace web.Controllers
{    
    [Route("api/user")]
    public class UserController : EntityController<User> 
    {
        public UserController(IRepository<User> repository) : base(repository) { }
    }

    [Route("api/cache/user")]
    public class UserCacheController : EntityCachedController<User>
    {
        public UserCacheController(IRepository<User> repository, ICacheRepository<User> cachedRepository) : base(repository, cachedRepository) { }
    }

    public class User : IdentityServer4.Models.IdentityResource, IEntity
    {
        public string Id { get; set;}
    }

    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private ICache _cache;
        private Microsoft.Extensions.Configuration.IConfiguration _config;
        public ConfigController(Microsoft.Extensions.Configuration.IConfiguration config,ICache cache) {
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

    }
}