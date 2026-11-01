namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class SimplePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    public required int Int1 { get; set; }
}

public class SimplePackerModelSerializer : ISerializer<SimplePackerModel>
{
    int ISerializer.IdClassType => 100;

    void ISerializer<SimplePackerModel>.Serialize(scoped in SimplePackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
        writer.Write(value.Int1);
    }
}

public class SimplePackerModelDeserializer : IDeserializer<SimplePackerModel>
{
    static int IDeserializer<SimplePackerModel>.IdClassType => 100;
    int IDeserializer.IdObject { get; set; }

    SimplePackerModel? IDeserializer<SimplePackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new SimplePackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString(),
            Int1 = reader.ReadInt32()
        };

        return value;
    }
}
