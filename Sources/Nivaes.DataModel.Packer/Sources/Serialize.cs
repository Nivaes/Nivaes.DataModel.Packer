using System;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer;

public static partial class DataModelPacker
{
    public static byte[] Serialize<T>(in T value)
        where T : IPackable<T>
    {
        var writer = new PackerWriter();
        value.Serialize(ref writer);

        return writer.ToArray();
    }
}
