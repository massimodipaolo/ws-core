using BenchmarkDotNet.Configs;

BenchmarkDotNet.Running.BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args /*, new DebugInProcessConfig()*/);

namespace benchmark
{
    public partial class Program
    {
    }
}
