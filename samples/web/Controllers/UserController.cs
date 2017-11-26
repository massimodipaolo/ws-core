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
        public UserCacheController(IRepository<User> repository, ICachedRepository<User> cachedRepository) : base(repository, cachedRepository) { }
    }

    public class User : IdentityServer4.Models.IdentityResource, IEntity
    {
        public string Id { get; set;}
    }
}