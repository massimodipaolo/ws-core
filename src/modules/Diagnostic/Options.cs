using Microsoft.AspNetCore.HttpLogging;
using StackExchange.Profiling;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Diagnostic;

public class Options : IOptions
{
    public Options() {
    }
    public ProfilerOptions? Profiler { get; set; } = new ProfilerOptions();
    public Ws.Core.Extensions.Diagnostic.Options.HttpLoggingOptions? HttpLogging { get; set; } = new Ws.Core.Extensions.Diagnostic.Options.HttpLoggingOptions();

    public class ProfilerOptions
    {
        public bool Enable { get; set; } = false;
        public MiniProfilerOptions Config { get; set; } = new MiniProfilerOptions();
    }

    public class HttpLoggingOptions
    {
        public bool Enable { get; set; } = false;
        public Microsoft.AspNetCore.HttpLogging.HttpLoggingOptions Config { get; set; } = new Microsoft.AspNetCore.HttpLogging.HttpLoggingOptions();
    }

}
