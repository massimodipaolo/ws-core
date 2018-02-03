using System;
using System.Collections.Generic;
using System.Text;
using core.Extensions.Base;

namespace core.Extensions.Data.Mongo
{
    public class Options : IOptions
    {
        public IEnumerable<Connection> Connections { get; set; }

        public class Connection
        {
            public string Name { get; set; } = "default";
            public string ConnectionString { get; set; }
            public string Database { get; set; }
        }
    }
}
