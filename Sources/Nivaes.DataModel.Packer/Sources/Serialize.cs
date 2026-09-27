namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static ReadOnlySpan<byte> Serialize<T>(in T value)
        where T : IPackable<T>
    {
        if (value == null)
            return [];

        var writer = new PackerWriter();
        writer.Write(value);

        writer.RecursiveWriter();

        return writer.ToSpan();
    }
}
