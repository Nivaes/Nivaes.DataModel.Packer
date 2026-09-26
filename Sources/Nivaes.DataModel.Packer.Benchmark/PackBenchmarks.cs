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

    private byte[]? _packSerialize;
    private byte[]? _memoryPackSerialize;
    private byte[]? _packReferenceSerialize;
    private byte[]? _memoryPackReferenceSerialize;

    [GlobalSetup]
    public void Setup()
    {
        _text1 = TextHelper.Create(Length);
        _text2 = TextHelper.Create(Length);
        _text3 = TextHelper.Create(Length);
        _text4 = TextHelper.Create(Length);

        _packSerialize = PreparePackSerialize();
        _memoryPackSerialize = PrepareMemoryPackSerialize();
        _packReferenceSerialize = PrivatePackReferenceSerialize();
        _memoryPackReferenceSerialize = PrepareReferenceMemoryPackSerialize();
    }

    private byte[] PreparePackSerialize()
    {
        var model = new SimplePackerModel
        {
            String1 = _text1,
            String2 = _text2,
        };

        return DataModelPacker.Serialize(model).ToArray();
    }

    [Benchmark]
    public void PackSerialize()
    {
        var model = PreparePackSerialize();
    }

    [Benchmark]
    public void PackDeserialize()
    {
        var copyModel = DataModelPacker.Deserialize<SimplePackerModel>(_packReferenceSerialize);
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
    public void MemoryPackSerialize()
    {
        var model = PrepareMemoryPackSerialize();
    }

    [Benchmark]
    public void MemoryPackDeserialize()
    {
        var copyModel = MemoryPackSerializer.Deserialize<SimpleMemoryPackModel>(_memoryPackSerialize);
    }

    private byte[] PrivatePackReferenceSerialize()
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
    public void PackReferenceSerialize()
    {
        var model = PrivatePackReferenceSerialize();
    }

    [Benchmark]
    public void PackReferenceDeserialize()
    {
        var copyModel = DataModelPacker.Deserialize<ReferencePackerModel>(_packSerialize);
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
    public void MemoryPackReferenceSerialize()
    {
        var model = PrepareReferenceMemoryPackSerialize();
    } 

    [Benchmark]
    public void MemoryReferencePackDeserialize()
    {
        var copyModel = MemoryPackSerializer.Deserialize<RootMemoryPackModel>(_memoryPackReferenceSerialize);
    }
}
