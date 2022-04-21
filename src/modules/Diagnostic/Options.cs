using Microsoft.AspNetCore.HttpLogging;
using StackExchange.Profiling;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Diagnostic;

public class Options : IOptions
{
    public Options() {
    }
    public MiniProfilerOptions? profiler { get; set; } = new MiniProfilerOptions();
    public HttpLoggingOptions? httpLogging { get; set; } = new HttpLoggingOptions();
}
