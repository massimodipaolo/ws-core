using System;
namespace Ws.Core.Extensions.Data
{
    public class DbConnection
    {
        public string Name { get; set; } = "default";
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
