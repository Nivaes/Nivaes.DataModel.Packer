using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MemoryPack;

namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class SimplePackBenchmarks
{
    [Benchmark]
    public void SimplePackSerialize()
    {
        var model = new SimplePackerModel
        {
            String1 = "test1",
            String2 = "test2",
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<SimplePackerModel>(cache);
    }

    [Benchmark]
    public void SimpleMemoryPackModel()
    {
        var model = new SimpleMemoryPackModel
        {
            String1 = "test1",
            String2 = "test2",
        };

        var cache = MemoryPackSerializer.Serialize(model);

        var copyModel = MemoryPackSerializer.Deserialize<SimpleMemoryPackModel>(cache);
    }

}
