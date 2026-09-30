using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveReader
{
    private delegate void DeserializeCircularDelegate(object item, ref PackerReader writer);

    private struct Entry
    {
        public object Item;
        public DeserializeCircularDelegate Deserializer;
    }

    private readonly ReadOnlySpan<byte> _buffer;
    private readonly ReadOnlySpan<int> _initPositions;
    private readonly Entry[] _entries;
    private int itemsRead;

    public RecursiveReader(ReadOnlySpan<byte> buffer)
    {
        _buffer = buffer;
        _initPositions = GetOffsets(buffer);
        _entries = new Entry[_initPositions.Length];
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

    public T? Read<T>(int id)
        where T : IPackable<T>
    {
        if (_entries[id].Item == null)
        {
            var position = _initPositions[id];
            var packerReader = new PackerReader(_buffer[position..], ref this);
            _entries[id] = new Entry {
                Item = T.Deserialize(ref packerReader)!,
                Deserializer = SerializeCircular<T>
            };
        }
       return (T?)_entries[id].Item;
    }

    public void DeserializeCircular()
    {
        int id = 0;
        foreach(var entry in _entries)
        {
            var position = _initPositions[id++];
            var packerReader = new PackerReader(_buffer[position..], ref this);
            entry.Deserializer(entry.Item, ref packerReader);
        }
    }

    private static void SerializeCircular<T>(
       object item, 
       ref PackerReader reader)
       where T : IPackable<T>
    {
        ((T)item).DeserializeCircular(ref reader);
    }
}
