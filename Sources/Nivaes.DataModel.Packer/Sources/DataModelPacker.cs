using System.Diagnostics.CodeAnalysis;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    static public Dictionary<int, Type> IdClassToType { get; } = new Dictionary<int, Type>();
    static public Dictionary<Type, Type> ClassToSerializerType { get; } = new Dictionary<Type, Type>();

    //public static ReadOnlySpan<byte> Serialize<T>(in T value)
    //{
    //    throw new NotImplementedException();
    //}

    public static ReadOnlySpan<byte> Serialize<T, TSerializer>(in T value)
        where TSerializer : IPackable<T>, new()
    {
        if (value == null)
            return [];

        var write = new PackerWriter();

        //return write.WriteRoot(value);
        return null;
    }

    //public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ReadOnlySpan<byte> buffer)
    //{
    //    throw new NotImplementedException();
    //}

    public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T, TSerializer>(ReadOnlySpan<byte> buffer)
           where TSerializer : IPackable<T>, new()
    {
        var recursiveReader = new RecursiveReader(buffer);
        var value = recursiveReader.Read<T,TSerializer>(0);

        recursiveReader.DeserializeCircular();

        //var reader = new PackerReader(buffer);
        //reader.ReadItems();


        //var value = reader.Read<T>();
        //var value = T.Deserialize(ref reader);

        //if (value != null)
        //{
        //    reader.Register(value);
        //    value.DeserializeCircular(ref reader);
        //}

        //var value = default(T);

        return value;
    }
}
