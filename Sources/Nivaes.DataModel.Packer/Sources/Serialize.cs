namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static ReadOnlySpan<byte> Serialize<T>(in T value)
        where T : IPackable<T>
    {
        var writer = new PackerWriter();
        value.Serialize(ref writer);

        writer.Close();

        return writer.ToSpan();
    }
}
