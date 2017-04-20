using System;
using System.Collections.Generic;

namespace core.Configuration
{
	public class Settings
	{
		public Settings()
		{
		}
		public IEnumerable<Db> DbList { get; set; }
		public ServerService Smtp { get; set; }

		public class Db
		{
            public string Connection { get; set; }
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