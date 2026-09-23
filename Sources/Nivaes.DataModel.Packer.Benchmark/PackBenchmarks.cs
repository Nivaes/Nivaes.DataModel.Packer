using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using MemoryPack;

namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
//[SimpleJob(RunStrategy.Throughput)]
public class PackBenchmarks
{
    //[Params(100, 1_000, 10_000, 100_000, 1_000_000)]
    [Params(100, 200)]
    public int Length;

    private string _text1 = null!;
    private string _text2 = null!;
    private string _text3 = null!;
    private string _text4 = null!;


    [GlobalSetup]
    public void Setup()
    {
        _text1 = TextHelper.Create(Length);
        _text2 = TextHelper.Create(Length);
        _text3 = TextHelper.Create(Length);
        _text4 = TextHelper.Create(Length);
    }

    [Benchmark]
    public void SimplePackSerialize()
    {
        var model = new SimplePackerModel
        {
            String1 = _text1,
            String2 = _text2,
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<SimplePackerModel>(cache);
    }

    [Benchmark]
    public void SimpleMemoryPackSerialize()
    {
        var model = new SimpleMemoryPackModel
        {
            String1 = _text1,
            String2 = _text2,
        };

        var cache = MemoryPackSerializer.Serialize(model);

        var copyModel = MemoryPackSerializer.Deserialize<SimpleMemoryPackModel>(cache);
    }

    [Benchmark]
    public void ReferencePackSerialize()
    {
        var model = new RootPackerModel
        {
            String1 = _text1,
            String2 = _text2,

            Reference = new ReferencePackerModel
            {
                String1 = _text3,
                String2 = _text4,
            }
        };

        var cache = DataModelPacker.Serialize(model);

        var copyModel = DataModelPacker.Deserialize<ReferencePackerModel>(cache);
    }

    [Benchmark]
    public void ReferenceMemoryPackSerialize()
    {
        var model = new RootMemoryPackModel
        {
            String1 = _text1,
            String2 = _text2,
            Reference = new ReferenceMemoryPackModel
            {
                String1 = _text3,
                String2 = _text4,
            }
        };

        var cache = MemoryPackSerializer.Serialize(model);

        var copyModel = MemoryPackSerializer.Deserialize<RootMemoryPackModel>(cache);
    }
}
