using BenchmarkDotNet.Running;
using Nivaes.DataModel.Packer.Benchmark.SimplePack;

namespace Nivaes.DataModel.Packer.Benchmark;

internal class Program
{
    static void Main(string[] args)
    {
        var _ = BenchmarkRunner.Run(typeof(SimplePackBenchmarks).Assembly);
    }
}
