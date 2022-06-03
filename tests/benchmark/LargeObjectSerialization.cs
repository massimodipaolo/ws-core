using BenchmarkDotNet.Attributes;

namespace benchmark
{
    [SimpleJob(warmupCount: 1, launchCount: 1, invocationCount: 2, targetCount: 10)]
    //[EventPipeProfiler(BenchmarkDotNet.Diagnosers.EventPipeProfile.CpuSampling)] // => https://https://www.speedscope.app/
    [MemoryDiagnoser]
    //[RPlotExporter]
    public class LargeObjectSerialization
    {
        [Params(xCore.EventLogQuery.Types.Source.Memory, xCore.EventLogQuery.Types.Source.Njson, xCore.EventLogQuery.Types.Source.Msjson, xCore.EventLogQuery.Types.Source.Buf)]
        public xCore.EventLogQuery.Types.Source Source;

        private static IEnumerable<xCore.EventLogItem> _data = xCore.Services.Data.GetEventLogDataApi(null, 100000, xCore.EventLogQuery.Types.Source.Memory);

        [Benchmark]
        public void Deserialize()
        {
            var items = xCore.Services.Data.GetEventLogDataApi(null, 1, Source);
            Console.WriteLine($"{Source} status: {items.Any()}");
        }

        [Benchmark]
        public void Serialize()
        {
            xCore.Services.Data.PostEventLogDataApi(_data, Source);
            Console.WriteLine($"{Source} serialized");
        }

    }
}