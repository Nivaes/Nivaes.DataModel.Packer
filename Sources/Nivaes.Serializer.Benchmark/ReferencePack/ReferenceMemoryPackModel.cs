using MemoryPack;

namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

[MemoryPackable]
public partial class ReferenceMemoryPackModel
{
    public string? String1 { get; set; }
    public string? String2 { get; set; }
}

[MemoryPackable]
public partial class RootMemoryPackModel
{
    public string? String1 { get; set; }
    public string? String2 { get; set; }

    public ReferenceMemoryPackModel? Reference { get; set;  }
}
