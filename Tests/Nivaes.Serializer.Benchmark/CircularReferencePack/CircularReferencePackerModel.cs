namespace Nivaes.Serializer.Benchmark.SimplePack;

[SerializePacker]
public class CircularReferencePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    public CircularRootPackerModel? Reference { get; set; }
}

[SerializePacker]
public class CircularRootPackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
    public required CircularReferencePackerModel Reference { get; set; }
}