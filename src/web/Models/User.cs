using System;
namespace web.Models
{
	public class User: Entity
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
	}
}
