using MemoryPack;

namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

[MemoryPackable]
public partial class SimpleMemoryPackModel 
{
    public string? String1 { get; set; }
    public string? String2 { get; set; }
}
