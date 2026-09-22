using System;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static byte[] Serialize<T>(in T value)
        where T : IPackable<T>
    {
        using var stream = new MemoryStream(1024);
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            value.Serialize(writer, value);
        }

        return stream.TryGetBuffer(out var buffer)
            ? buffer.AsSpan(0, checked((int)stream.Length)).ToArray()
            : stream.ToArray();
    }
}
