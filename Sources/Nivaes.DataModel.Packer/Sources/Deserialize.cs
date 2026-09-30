using System.Diagnostics.CodeAnalysis;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ReadOnlySpan<byte> buffer)
          where T : IPackable<T>
    {
        var recursiveReader = new RecursiveReader(buffer);
        var value = recursiveReader.Read<T>(0);

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
