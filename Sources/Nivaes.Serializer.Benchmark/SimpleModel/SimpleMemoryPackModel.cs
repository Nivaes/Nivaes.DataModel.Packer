using MemoryPack;

namespace Nivaes.Serializer.Benchmark.SimplePack;

[MemoryPackable]
public partial class SimpleMemoryPackModel 
{
    public string? String1 { get; set; }
    public string? String2 { get; set; }
    public int Int1 { get; set; }
}
