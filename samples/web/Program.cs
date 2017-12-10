using Microsoft.AspNetCore.Hosting;

namespace web
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = core.Program.WebHostBuilder(args, typeof(Program).Assembly)
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
