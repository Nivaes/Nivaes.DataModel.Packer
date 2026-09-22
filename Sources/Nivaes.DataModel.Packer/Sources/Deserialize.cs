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
        var reader = new PackerReader(buffer);

        return T.Deserialize(ref reader);
    }
}
