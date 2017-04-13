using System;
using web.Data;
using Microsoft.AspNetCore.Mvc;
namespace web.Controllers
{

    public class UserController : EntityController<User> //EntityCachedController<User>
    {
        public UserController(IRepository<User> repository) : base(repository) { }
    }
}