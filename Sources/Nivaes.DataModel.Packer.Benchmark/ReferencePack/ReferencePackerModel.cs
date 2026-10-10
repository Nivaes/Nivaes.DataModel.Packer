namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class ReferencePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
}

public class ReferencePackerModelSerializer : IPackable<ReferencePackerModel>
{
    int IPackable<ReferencePackerModel>.IdClass => 1;

    void IPackable<ReferencePackerModel>.Serialize(ReferencePackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
    }

    ReferencePackerModel? IPackable<ReferencePackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new ReferencePackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString()
        };

        return value;
    }

    public void DeserializeCircular(ref PackerReader reader)
    {
    }
}

public class RootPackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
    public required ReferencePackerModel Reference { get; set; }
}

public class RootPackerModelSerializer : IPackable<RootPackerModel>
{
    int IPackable<RootPackerModel>.IdClass => 102;

    void IPackable<RootPackerModel>.Serialize(RootPackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
        //writer.Write(value.Reference);
    }

    RootPackerModel? IPackable<RootPackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new RootPackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString(),
            //Reference = reader.Read<ReferencePackerModel>()!
            Reference = (ReferencePackerModel?)null!
        };

        return value;
    }

    void IPackable<RootPackerModel>.DeserializeCircular(ref PackerReader reader)
    {
    }
}
