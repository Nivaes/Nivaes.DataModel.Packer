namespace Nivaes.Serializer.Benchmark.SimplePack;

[SerializePacker]
public class ReferencePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
}

[SerializePacker]
public class RootPackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
    public required ReferencePackerModel Reference { get; set; }
}