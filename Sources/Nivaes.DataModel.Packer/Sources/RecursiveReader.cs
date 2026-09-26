using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private int[] initPositions;

    public RecursiveReader(ReadOnlySpan<byte> buffer)
    {
        _buffer = buffer;
        initPositions = GetOffsets(buffer);
    }

    public static int[] GetOffsets(ReadOnlySpan<byte> buffer)
    {
        var result = new int[buffer.Length / 4];

        int count = 0;
        int position = 0;

        while (position + 4 <= buffer.Length)
        {
            result[count++] = position;

            int jump = BinaryPrimitives.ReadInt32LittleEndian(
                buffer.Slice(position, 4));

            if (jump <= 0)
                break;

            position += jump;
        }

        return result[..count];
    }

    public T? Read<T>(int id, ref PackerReader reader)
        where T : IPackable<T>
    {
        var initPosition = initPositions[id];
        ReadOnlySpan<byte> remaining = _buffer.Slice(initPosition);

        return T.Deserialize(ref reader);
    }
}
