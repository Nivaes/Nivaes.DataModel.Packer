using System.Diagnostics.CodeAnalysis;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

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
