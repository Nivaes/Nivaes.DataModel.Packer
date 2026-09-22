using BenchmarkDotNet.Running;

namespace Nivaes.SerializerBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var _ = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
