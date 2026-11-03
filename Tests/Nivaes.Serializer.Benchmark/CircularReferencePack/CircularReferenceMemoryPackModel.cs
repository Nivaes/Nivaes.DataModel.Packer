using MemoryPack;

namespace Nivaes.Serializer.Benchmark.SimplePack;

[MemoryPackable(GenerateType.CircularReference)]
public partial class CircularReferenceMemoryPackModel
{
    [MemoryPackOrder(0)]
    public string? String1 { get; set; }
    [MemoryPackOrder(1)]
    public string? String2 { get; set; }

    [MemoryPackOrder(2)]
    public CircularRootMemoryPackModel? Reference { get; set; }
}

[MemoryPackable(GenerateType.CircularReference)]
public partial class CircularRootMemoryPackModel
{
    [MemoryPackOrder(0)]
    public string? String1 { get; set; }
    [MemoryPackOrder(1)]
    public string? String2 { get; set; }

    [MemoryPackOrder(2)]
    public CircularReferenceMemoryPackModel? Reference { get; set;  }
}
