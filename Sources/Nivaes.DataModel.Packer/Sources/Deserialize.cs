using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ReadOnlySpan<byte> buffer)
          where T : IPackable<T>
    {
        var reader = new PackerReader(buffer);
        //reader.ReadItems();


        var value = reader.Read<T>();
        //var value = T.Deserialize(ref reader);

        //if (value != null)
        //{
        //    reader.Register(value);
        //    value.DeserializeCircular(ref reader);
        //}

        return value;
    }
}
