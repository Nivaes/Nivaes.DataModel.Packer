namespace Nivaes.Serializer.Benchmark.SimplePack;

public class SimpleNivaesSerializeModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    public required int Int1 { get; set; }
}

public class SimplePackerModelSerializer : ISerializer<SimpleNivaesSerializeModel>
{
    void ISerializer<SimpleNivaesSerializeModel>.Serialize(scoped in SimpleNivaesSerializeModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
        writer.Write(value.Int1);
    }
}

public class SimplePackerModelDeserializer : IDeserializer<SimpleNivaesSerializeModel>
{
    SimpleNivaesSerializeModel? IDeserializer<SimpleNivaesSerializeModel>.Deserialize(ref PackerReader reader)
    {
        var value = new SimpleNivaesSerializeModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString(),
            Int1 = reader.ReadInt32()
        };

        return value;
    }
}
