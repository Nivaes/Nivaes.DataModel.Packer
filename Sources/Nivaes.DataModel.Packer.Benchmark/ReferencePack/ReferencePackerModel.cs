namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class ReferencePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
}

public class ReferencePackerModelSerializer : ISerializer<ReferencePackerModel>
{
    void ISerializer<ReferencePackerModel>.Serialize(scoped in ReferencePackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
    }
}

public class ReferencePackerModelDeserializer : IDeserializer<ReferencePackerModel>
{
    ReferencePackerModel? IDeserializer<ReferencePackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new ReferencePackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString()
        };

        return value;
    }
}

public class RootPackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }
    public required ReferencePackerModel Reference { get; set; }
}

public class RootPackerModelSerializer : ISerializer<RootPackerModel>
{
    void ISerializer<RootPackerModel>.Serialize(scoped in RootPackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
        writer.Write<ReferencePackerModel, ReferencePackerModelSerializer>(value.Reference);
    }
}
public class RootPackerModelDeserializer : IDeserializer<RootPackerModel>
{
    RootPackerModel? IDeserializer<RootPackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new RootPackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString(),
            Reference = reader.Read<RootPackerModel, ReferencePackerModel, ReferencePackerModelDeserializer>()!
        };

        return value;
    }
}
