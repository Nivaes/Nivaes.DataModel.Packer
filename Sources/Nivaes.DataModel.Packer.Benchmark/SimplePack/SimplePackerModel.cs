namespace Nivaes.DataModel.Packer.Benchmark.SimplePack;

public class SimplePackerModel : IPackable<SimplePackerModel>
{
    public required string String1 { get; set; }
    public required string String2 { get; set; }

    void IPackable<SimplePackerModel>.Serialize(ref PackerWriter writer)
    {
        writer.Write(String1);
        writer.Write(String2);
    }

    static SimplePackerModel? IPackable<SimplePackerModel>.Deserialize(ref PackerReader reader)
    {
        var value = new SimplePackerModel
        {
            String1 = reader.ReadString(),
            String2 = reader.ReadString()
        };

        return value;
    }
}
