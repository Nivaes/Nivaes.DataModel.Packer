using System;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static byte[] Serializer<T>(in T value)
        where T : IPackable<T>
    {
        var packable = (IPackable<T>)value;
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            value.Serialize(writer, value);
        }

        return stream.ToArray();
    }
}
