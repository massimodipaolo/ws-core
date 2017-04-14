using System;
using System.Collections.Generic;

namespace web.Configuration
{
	public class Settings
	{
		public Settings()
		{
		}
		public IEnumerable<Db> DbList { get; set; }
		public ServerService Smtp { get; set; }

		public class Db : ServerService
		{
			public string Name { get; set; }
			public Types Type { get; set; } = Types.Memory;
			public enum Types
			{
				Memory,
				FileSystem,
				Mongo,
				SqlServer
			}
		}

		public class ServerService
		{
			public string Host { get; set; }
			public int Port { get; set; }
			public string User { get; set; }
			public string Psw { get; set; }
		}


	}


}