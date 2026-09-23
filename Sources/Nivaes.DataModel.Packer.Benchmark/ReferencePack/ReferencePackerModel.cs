namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class ReferencePackerModel : IPackable<ReferencePackerModel>
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    void IPackable<ReferencePackerModel>.Serialize(ref PackerWriter writer)
    {
        writer.Write(String1);
        writer.Write(String2);
    }

    static ReferencePackerModel? IPackable<ReferencePackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new ReferencePackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString()
        };

        return value;
    }
}

public class RootPackerModel : IPackable<RootPackerModel>
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    public required ReferencePackerModel Reference { get; set; }

    void IPackable<RootPackerModel>.Serialize(ref PackerWriter writer)
    {
        writer.Write(String1);
        writer.Write(String2);
        writer.Write(Reference);
    }

    static RootPackerModel? IPackable<RootPackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new RootPackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString(),
            Reference = reader.Reader<ReferencePackerModel>()!
        };

        return value;
    }
}
