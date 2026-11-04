using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Nivaes.Serializer.Benchmark.SimplePack;

namespace Nivaes.Serializer.Benchmark;

internal class Program
{
    static void Main(string[] args)
    {
        _ = BenchmarkRunner.Run<PackBenchmarks>();
        //_ = BenchmarkRunner.Run<PackBenchmarks>(new DebugInProcessConfig());

        //var config = DefaultConfig.Instance
        //    .AddJob(Job.Default
        //        .WithToolchain(InProcessNoEmitToolchain.Instance));

        //var config = DefaultConfig.Instance
        //   .AddJob(Job.Default);

        //_ = BenchmarkRunner.Run<PackBenchmarks>(config);
    }
}
