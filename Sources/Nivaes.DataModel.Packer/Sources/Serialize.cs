namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static ReadOnlySpan<byte> Serialize<T>(in T value)
        where T : IPackable<T>
    {
        if (value == null)
            return [];

        var recursiveWrite = new RecursiveWriter();

        return recursiveWrite.WriteRoot(value);
    }
}
