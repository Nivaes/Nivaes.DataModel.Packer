using System.Diagnostics.CodeAnalysis;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static ReadOnlySpan<byte> Serialize<T, TSerializer>(in T value)
        where TSerializer : ISerializer<T>, new()
    {
        if (value == null)
            return [];

        var write = new PackerWriter();
        var serializer = new TSerializer();

        return write.WriteRoot(value, serializer);
    }

    public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T, TDeserializer>(ReadOnlySpan<byte> buffer)
           where TDeserializer : IDeserializer<T>, new()
    {
        var recursiveReader = new RecursiveReader(buffer);
        var serializer = new TDeserializer();

        var value = recursiveReader.Read<T, T, TDeserializer>(0);

        recursiveReader.DeserializeCircular<T>();

        return value;
    }
}
