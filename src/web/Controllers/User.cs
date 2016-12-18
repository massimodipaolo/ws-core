using System;
using web.Data;
using Microsoft.AspNetCore.Mvc;
namespace web.Controllers
{
	[Route("api/[controller]")]
	public class UserController: EntityController<User>
	{
		public UserController(IRepository<User> repository): base(repository) {}
	}
}
