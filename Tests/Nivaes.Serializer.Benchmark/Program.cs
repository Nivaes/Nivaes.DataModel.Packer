using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Nivaes.Serializer.Benchmark.SimplePack;

namespace Nivaes.Serializer.Benchmark;

internal class Program
{
    static void Main(string[] args)
    {
        _ = BenchmarkRunner.Run<PackBenchmarks>();
        //_ = BenchmarkRunner.Run<PackBenchmarks>(new DebugInProcessConfig());
    }
}
