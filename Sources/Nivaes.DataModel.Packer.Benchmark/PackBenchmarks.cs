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
    [Params(100, 10_000)]
    public int Length;

    private string _text1 = null!;
    private string _text2 = null!;
    private string _text3 = null!;
    private string _text4 = null!;

    private byte[]? _serializePackSerialize;
    private byte[]? _memoryPackSerialize;
    private byte[]? _referencePackSerialize;
    private byte[]? _referenceMemoryPackSerialize;

    [GlobalSetup]
    public void Setup()
    {
        _text1 = TextHelper.Create(Length);
        _text2 = TextHelper.Create(Length);
        _text3 = TextHelper.Create(Length);
        _text4 = TextHelper.Create(Length);

        _serializePackSerialize = PrepareSerializePackSerialize();
        _memoryPackSerialize = PrepareMemoryPackSerialize();
        _referencePackSerialize = PrivateReferencePackSerialize();
        _referenceMemoryPackSerialize = PrepareReferenceMemoryPackSerialize();
    }

    private byte[] PrepareSerializePackSerialize()
    {
        var model = new SimplePackerModel
        {
            String1 = _text1,
            String2 = _text2,
        };

        return DataModelPacker.Serialize(model).ToArray();
    }

    [Benchmark]
    public void SerializePackSerialize()
    {
        PrepareSerializePackSerialize();
    }

    private byte[] PrepareMemoryPackSerialize()
    {
        var model = new SimpleMemoryPackModel
        {
            String1 = _text1,
            String2 = _text2,
        };

        return MemoryPackSerializer.Serialize(model);
    }

    [Benchmark]
    public void SimpleMemoryPackSerialize()
    {
        PrepareMemoryPackSerialize();
    }

    private byte[] PrivateReferencePackSerialize()
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

        return DataModelPacker.Serialize(model).ToArray();
    }

    [Benchmark]
    public void ReferencePackSerialize()
    {
        var model = PrivateReferencePackSerialize();
    }

    private byte[] PrepareReferenceMemoryPackSerialize()
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

        return MemoryPackSerializer.Serialize(model);
    }

    [Benchmark]
    public void ReferenceMemoryPackSerialize()
    {
        var model = PrepareReferenceMemoryPackSerialize();
    }

    [Benchmark]
    public void ReferencePackDeserialize()
    {
        var copyModel = DataModelPacker.Deserialize<ReferencePackerModel>(_serializePackSerialize);
    }

    [Benchmark]
    public void SimpleMemoryPackDeserialize()
    {
        var copyModel = MemoryPackSerializer.Deserialize<SimpleMemoryPackModel>(_memoryPackSerialize);
    }

    [Benchmark]
    public void SimplePackDeserialize()
    {
        var copyModel = DataModelPacker.Deserialize<SimplePackerModel>(_referencePackSerialize);
    }

    [Benchmark]
    public void ReferenceMemoryPackDeserialize()
    {
        var copyModel = MemoryPackSerializer.Deserialize<RootMemoryPackModel>(_referenceMemoryPackSerialize);
    }
}
