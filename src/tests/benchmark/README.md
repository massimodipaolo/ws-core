# benchmark

Benchmark / stress load tool
[Docs](https://github.com/dotnet/BenchmarkDotNet)

## build

Rebuild project via pwsh
```powershell
    dotnet build .\benchmark.csproj -c Release --no-incremental
```

## debugging

On Program.cs add Debug parm
```csharp
BenchmarkDotNet.Running.BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args, new DebugInProcessConfig());    
```