using System;
using core.Data;
using core.Models;
using Microsoft.AspNetCore.Mvc;
namespace core.Controllers
{

    public class UserController : EntityController<User> //EntityCachedController<User>
    {
        public UserController(IRepository<User> repository) : base(repository) { }
    }
}