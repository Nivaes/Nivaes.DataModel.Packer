using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MemoryPack;

namespace Nivaes.Serializer.Benchmark.SimplePack;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
//[SimpleJob(RunStrategy.Throughput)]
[SimpleJob(launchCount: 1, warmupCount: 0, iterationCount: 1)]
public class PackBenchmarks
{
    //[Params(100, 1_000, 10_000, 100_000, 1_000_000)]
    [Params(100, 1_000)]
    public int Length;

    private string _text1 = null!;
    private string _text2 = null!;
    private string _text3 = null!;
    private string _text4 = null!;

    private byte[]? _packSerialize;
    private byte[]? _memoryPackSerialize;
    private byte[]? _packReferenceSerialize;
    private byte[]? _memoryPackReferenceSerialize;

    private byte[]? _packCircularReferenceSerialize;
    private byte[]? _memoryPackCircularReferenceSerialize;

    [GlobalSetup]
    public void Setup()
    {
        _text1 = TextHelper.Create(Length);
        _text2 = TextHelper.Create(Length);
        _text3 = TextHelper.Create(Length);
        _text4 = TextHelper.Create(Length);

        _packSerialize = PrepareNivaesSerialize();
        _memoryPackSerialize = PrepareMemoryPackSerialize();
        
        _packReferenceSerialize = PrepareNivaesReferenceSerialize();
        _memoryPackReferenceSerialize = PrepareReferenceMemoryPackSerialize();

        _packCircularReferenceSerialize = PrepareNivaesCircularReferenceSerialize();
        _memoryPackCircularReferenceSerialize = PrepareCircularReferenceMemoryPackSerialize();
    }

    #region Serializer
    private byte[] PrepareNivaesSerialize()
    {
        var model = new SimpleNivaesSerializeModel
        {
            String1 = _text1,
            String2 = _text2,
            Int1 = 42
        };

        return SerializePacker.Serialize(model).ToArray();
    }

    [Benchmark]
    public void NivaesSerialize()
    {
        var model = PrepareNivaesSerialize();
    }

    [Benchmark]
    public void NivaesDeserialize()
    {
        var copyModel = SerializePacker.Deserialize<SimpleNivaesSerializeModel>(_packSerialize);
    }

    private byte[] PrepareMemoryPackSerialize()
    {
        var model = new SimpleMemoryPackModel
        {
            String1 = _text1,
            String2 = _text2,
            Int1 = 42
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
    #endregion

    #region Reference
    private byte[] PrepareNivaesReferenceSerialize()
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

        return SerializePacker.Serialize(model).ToArray();
    }

    [Benchmark]
    public void NivaesReferenceSerialize()
    {
        var model = PrepareNivaesReferenceSerialize();
    }

    [Benchmark]
    public void NivaesReferenceDeserialize()
    {
        var copyModel = SerializePacker.Deserialize<RootPackerModel>(_packReferenceSerialize);
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
    public void MemoryPackReferenceDeserialize()
    {
        var copyModel = MemoryPackSerializer.Deserialize<RootMemoryPackModel>(_memoryPackReferenceSerialize);
    }
    #endregion

    #region Circular Reference
    private byte[] PrepareNivaesCircularReferenceSerialize()
    {
        var model = new CircularRootPackerModel
        {
            String1 = _text1,
            String2 = _text2,

            Reference = new CircularReferencePackerModel
            {
                String1 = _text3,
                String2 = _text4,
            }
        };

        model.Reference.Reference = model;

        return SerializePacker.Serialize(model).ToArray();
    }

    [Benchmark]
    public void NivaesCircularReferenceSerialize()
    {
        var model = PrepareNivaesCircularReferenceSerialize();
    }

    [Benchmark]
    public void NivaesCircularReferenceDeserialize()
    {
        var copyModel = SerializePacker.Deserialize<CircularRootPackerModel>(_packCircularReferenceSerialize);
    }

    private byte[] PrepareCircularReferenceMemoryPackSerialize()
    {
        var model = new CircularRootMemoryPackModel
        {
            String1 = _text1,
            String2 = _text2,
            Reference = new CircularReferenceMemoryPackModel
            {
                String1 = _text3,
                String2 = _text4,
            }
        };

        model.Reference.Reference = model;

        return MemoryPackSerializer.Serialize(model);
    }

    [Benchmark]
    public void MemoryPackCircularReferenceSerialize()
    {
        var model = PrepareCircularReferenceMemoryPackSerialize();
    }

    [Benchmark]
    public void MemoryPackCircularReferenceDeserialize()
    {
        var copyModel = MemoryPackSerializer.Deserialize<CircularRootMemoryPackModel>(_memoryPackCircularReferenceSerialize);
    }
    #endregion
}
