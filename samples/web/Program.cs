using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace web
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = core.Program.WebHostBuilder(args)
                .UseStartup<Startup>()
                .Build();            
            host.Run();
        }
    }
}
