using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Nivaes.DataModel.Packer.Benchmark.SimplePack;

namespace Nivaes.DataModel.Packer.Benchmark;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<PackBenchmarks>();
        //_ = BenchmarkRunner.Run<PackBenchmarks>(new DebugInProcessConfig());
    }
}
