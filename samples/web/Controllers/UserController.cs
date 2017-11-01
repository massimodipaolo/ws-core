using System;
using core.Extensions.Data;
using Microsoft.AspNetCore.Mvc;
using core.Extensions.Api.Controllers;

namespace web.Controllers
{    
    public class UserController : EntityController<User> //EntityCachedController<User>
    {
        public UserController(IRepository<User> repository) : base(repository) { }
    }

    public class User : IdentityServer4.Models.IdentityResource, IEntity
    {
        public string Id { get; set;}
    }
}