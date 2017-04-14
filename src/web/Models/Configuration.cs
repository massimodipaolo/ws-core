using System;
using System.Collections.Generic;

namespace web.Configuration
{
	public class Settings
	{
		public Settings()
		{
		}
		public IEnumerable<Db> Db { get; set; }
		public Smtp Smtp { get; set; }
    }

    public class Db : ServerService
    {
        public string Name { get; set; }
        public Types Type { get; set; }
        public enum Types
        {
            Memory,
            FileSystem,
            Mongo,
            SqlServer
        }
    }

    public class Smtp : ServerService
    {
    }

    public class ServerService
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Psw { get; set; }
    }
}
