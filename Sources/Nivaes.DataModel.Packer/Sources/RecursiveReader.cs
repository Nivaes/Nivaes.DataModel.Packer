using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveReader
{
    private readonly ReadOnlySpan<byte> _buffer;
    private readonly ReadOnlySpan<int> _initPositions;
    private readonly object[] _items;
    private int itemsRead;

    public RecursiveReader(ReadOnlySpan<byte> buffer)
    {
        _buffer = buffer;
        _initPositions = GetOffsets(buffer);
        _items = new object[_initPositions.Length];
    }

    public static int[] GetOffsets(ReadOnlySpan<byte> buffer)
    {
        var result = new int[buffer.Length / 4];

        int count = 0;
        int position = 0;

        while (position + 4 <= buffer.Length)
        {
            result[count++] = position + 4;

            int jump = BinaryPrimitives.ReadInt32LittleEndian(
                buffer.Slice(position, 4));

            if (jump <= 0)
                break;

            position += jump;
        }

        return result[..count];
    }

    //internal void ReadItems(ref PackerReader reader)
    //{
    //    foreach(var position in _initPositions)
    //    {
    //        var packerReader = new PackerReader(_buffer[position..]);
    //    }
    //}

    //internal void Register<T>(T item)
    //    where T : IPackable<T>
    //{
    //    _items[itemsRead++] = item;
    //}

    public T? Read<T>(/*int id,*/ ref PackerReader reader)
        where T : IPackable<T>
    {
        //var initPosition = _initPositions[id];
        //ReadOnlySpan<byte> remaining = _buffer.Slice(initPosition);

        //if (_items[id] != null) 
        //    return (T)_items[id];

        //var positon = _initPositions[];
        //var reader = new PackerReader(_buffer[]);

        var position = _initPositions[itemsRead++];

        var packerReader = new PackerReader(_buffer[position..], ref this);

        var item = T.Deserialize(ref packerReader);

        //if(item != null)
        //    Register<T>(item!);

        //if (item != null)
        //    item.DeserializeCircular(ref reader);

        return item;
    }
}
