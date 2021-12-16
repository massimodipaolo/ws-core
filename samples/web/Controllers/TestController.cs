using System;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;
using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Api.Controllers;
using System.Collections.Generic;
using web.Code;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace web.Controllers
{

    public class PageController : EntityControllerWithMethods<Page, int>
    {
        public PageController(IRepository<Page, int> repository) : base(repository) { }

        [HttpGet]
        [Route("filter")]
        public IActionResult ListFilter()
        {
            return Ok(_repository.List.Where(_ => _.Title.Any(__ => __.Text.StartsWith("Luca"))));
        }

    }


    /// <summary>
    /// Awesome user controller
    /// </summary>
    [Route("api/user")]
    public class UserController : EntityControllerWithMethods<User, Guid>
    {
        public UserController(IRepository<User, Guid> repository) : base(repository) { }
    }

    /// <summary>
    /// Cached version of the Awesome user controller
    /// </summary>
    [Route("api/cache/user")]
    public class UserCacheController : EntityCachedControllerWithMethods<User, Guid>
    {
        public UserCacheController(IRepository<User, Guid> repository, ICacheRepository<User, Guid> cachedRepository) : base(repository, cachedRepository) { }
    }

    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private ICache _cache;
        private Dictionary<string, string> _config;
        public ConfigController(Microsoft.Extensions.Configuration.IConfiguration config, ICache cache)
        {
            _config = config.AsEnumerable()
                        .Select(conf => new
                        {
                            conf.Key,
                            Value = new string[] { "connectionstring", "username", "password", "pwd", "secret", "apikey" }.Any(s => conf.Key.ToLower().Contains(s))
                                ?
                                    new string('*', 8)
                                :
                                conf.Value
                        })
                        .OrderBy(conf => conf.Key)
                        .ToDictionary(conf => conf.Key, conf => conf.Value);
            _cache = cache;
            Ws.Core.AppInfo<AppConfig>.LoggerFactory.CreateLogger(nameof(ConfigController)).LogInformation("ConfigController ctor");
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

    [Route("hook")]
    public class HookController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] object data, [FromServices]Ws.Core.Extensions.Message.IMessage notify)
        {
            var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<HookMessage>(data.ToString());
            if (payload != null)
            {
                var message = new Ws.Core.Extensions.Message.Message()
                {
                    Subject = payload.Title,
                    Content = payload.Text,
                    Arguments = payload.IsFailure ? new { Importance = "High" } : null,
                    Sender = new Ws.Core.Extensions.Message.Message.Actor() { Address = "no-reply@web.local" },
                    Format = "text",
                    Recipients = new Ws.Core.Extensions.Message.Message.Actor[]
    {
                    new Ws.Core.Extensions.Message.Message.Actor() {
                        Address = "operator@web.local",
                        Name="DTC",
                        Type= Ws.Core.Extensions.Message.Message.ActorType.Primary}
    }
                };
                notify.SendAsync(message);
                return Ok();
            }
            return BadRequest();
        }
    }
    public class HookMessage
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsFailure { get; set; } = false;
    }
}