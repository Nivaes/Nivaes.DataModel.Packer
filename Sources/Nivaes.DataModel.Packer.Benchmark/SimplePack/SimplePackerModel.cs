namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class SimplePackerModel
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    public required int Int1 { get; set; }
}

public class SimplePackerModelSerializer : IPacker<SimplePackerModel>
{
    int IPacker<SimplePackerModel>.IdClassType => 100;

    void IPacker<SimplePackerModel>.Serialize(scoped in SimplePackerModel value, ref PackerWriter writer)
    {
        writer.Write(value.String1);
        writer.Write(value.String2);
        writer.Write(value.Int1);
    }

    SimplePackerModel? IPacker<SimplePackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new SimplePackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString(),
            Int1 = reader.ReadInt32()
        };

        return value;
    }

    //void IPacker<SimplePackerModel>.DeserializeCircular(SimplePackerModel value, ref PackerReader reader)
    //{
    //}
}
