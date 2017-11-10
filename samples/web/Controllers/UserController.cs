using System;
using core.Extensions.Data;
using core.Extensions.Cache;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Api.Controllers;

namespace web.Controllers
{    
    public class UserController : EntityCachedController<User> //EntityCachedController<User>
    {
        public UserController(IRepository<User> repository,ICachedRepository<User> cachedRepository) : base(repository,cachedRepository) { }
    }

    public class User : IdentityServer4.Models.IdentityResource, IEntity
    {
        public string Id { get; set;}
    }
}