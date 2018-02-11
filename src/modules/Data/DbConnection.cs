using System;
namespace core.Extensions.Data
{
    public class DbConnection
    {
        public string Name { get; set; } = "default";
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
