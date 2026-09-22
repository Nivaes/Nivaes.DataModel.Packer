using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static T? Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(byte[] buffer /*ReadOnlySpan<byte> buffer*/)
          where T : IPackable<T>
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        using (var writer = new BinaryReader(stream, Encoding.UTF8))
        {
            return T.Deserialize(writer);
        }
    }
}
