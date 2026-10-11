namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class ReferencePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
}

public class ReferencePackerModelSerializer : IPacker<ReferencePackerModel>
{
    int IPacker<ReferencePackerModel>.IdClassType => 1;

    void IPacker<ReferencePackerModel>.Serialize(scoped in ReferencePackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
    }

    ReferencePackerModel? IPacker<ReferencePackerModel>.Deserialize(ref PackerReader reader)
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

public class RootPackerModelSerializer : IPacker<RootPackerModel>
{
    int IPacker<RootPackerModel>.IdClassType => 102;

    void IPacker<RootPackerModel>.Serialize(scoped in RootPackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
        //writer.Write(value.Reference);
    }

    RootPackerModel? IPacker<RootPackerModel>.Deserialize(ref PackerReader reader)
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

    void IPacker<RootPackerModel>.DeserializeCircular(ref PackerReader reader)
    {
    }
}
