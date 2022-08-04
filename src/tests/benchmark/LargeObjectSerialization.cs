using BenchmarkDotNet.Attributes;

namespace benchmark;

[SimpleJob(warmupCount: 1, launchCount: 1, invocationCount: 2, targetCount: 10)]
//[EventPipeProfiler(BenchmarkDotNet.Diagnosers.EventPipeProfile.CpuSampling)] // => https://https://www.speedscope.app/
[MemoryDiagnoser]
//[RPlotExporter]
public class LargeObjectSerialization
{
    [Params(x.core.EventLogQuery.Types.Source.Memory, x.core.EventLogQuery.Types.Source.Njson, x.core.EventLogQuery.Types.Source.Msjson, x.core.EventLogQuery.Types.Source.Buf)]
    public x.core.EventLogQuery.Types.Source Source;

    private static readonly IEnumerable<x.core.EventLogItem> _data = x.core.Services.Data.GetEventLogDataApi(100000, x.core.EventLogQuery.Types.Source.Memory);

    [Benchmark]
    public void Deserialize()
    {
        var items = x.core.Services.Data.GetEventLogDataApi(1, Source);
        Console.WriteLine($"{Source} status: {items.Any()}");
    }

    [Benchmark]
    public void Serialize()
    {
        x.core.Services.Data.PostEventLogDataApi(_data, Source);
        Console.WriteLine($"{Source} serialized");
    }

}