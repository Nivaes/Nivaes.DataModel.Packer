namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static ReadOnlySpan<byte> Serialize<T>(in T value)
    {
        throw new NotImplementedException();
    }

    public static ReadOnlySpan<byte> Serialize<T, TSerializer>(in T value)
        where TSerializer : IPackable<T>, new()
    {
        if (value == null)
            return [];

        var write = new PackerWriter();

        //return write.WriteRoot(value);
        return null;
    }
}
