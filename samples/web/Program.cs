using Microsoft.AspNetCore.Hosting;

namespace web
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Ws.Core.NLog.Program.Configure<AppConfig>();
            Ws.Core.NLog.Program.Main(args);
        }
    }
}
