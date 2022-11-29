using BenchmarkDotNet.Attributes;

namespace benchmark;

[ShortRunJob]
[EventPipeProfiler(BenchmarkDotNet.Diagnosers.EventPipeProfile.CpuSampling)] // => https://https://www.speedscope.app/
[MemoryDiagnoser]
//[RPlotExporter]
public class PayloadRequest
{
    //[Params(false,true)]
    //public bool parallel;
    //[Params(false,true)]
    //public bool decompression = true;

    private static bool _init = false;
    public PayloadRequest()
    {
        if (!_init)
        {
            ws.bom.oven.web.services.PayloadCms._gateway =
                new ws.bom.oven.web.code.AppConfig.GatewayConfig.PayloadCmsConfig()
                {
                    Host = "http://localhost:4000",
                    UserName = "admin@bowl-payload.com",
                    Password = "admin",
                    Slugs = new()
                    {
                        Auth = "users",
                        Category = "category",
                        Market = "market",
                        Locale = "language",
                        ExcludeFromStore = new string[] {"media", "page" }
                    }
                };
            _ = new ws.bom.oven.web.services.PayloadCms();
            _init = true;
        }
    }
    
    [Benchmark]
    public async Task Store()
    {
        var rs = await ws.bom.oven.web.services.PayloadCms.Store();
        Console.WriteLine($"rs length: {rs?.Length}");
    }


}