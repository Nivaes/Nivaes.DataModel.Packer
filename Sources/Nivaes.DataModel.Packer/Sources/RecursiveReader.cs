using System.Buffers.Binary;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveReader
{
    private delegate void DeserializeCircularDelegate(object item, scoped in object serializer, ref PackerReader reader);

    private struct Entry
    {
        public bool Create;
        public object Item;
        public DeserializeCircularDelegate DeserializerCircular;
    }

    private readonly ReadOnlySpan<byte> _buffer;
    private readonly ReadOnlySpan<int> _initPositions;
    private readonly Entry[] _entries;

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

    public T? Read<T, TSerializer>(int id)
        where TSerializer : IPacker<T>, new()
    {
        var serializer = new TSerializer();
        if(!_entries[id].Create)
        {
            var position = _initPositions[id];
            var packerReader = new PackerReader(_buffer[position..], ref this);
            var idClassType = packerReader.ReadInt32(); 
            if(idClassType != serializer.IdClassType)
            {
                throw new Exception($"No coinciden las clase: {idClassType} != {serializer.IdClassType}");
            }
            var idObject = packerReader.ReadInt32();
            if (idObject!= id) 
            {
                throw new Exception($"No coinciden el id objet: {idObject} != {id}");
            }

            _entries[id].Create = true;
            _entries[id].Item = serializer.Deserialize(ref packerReader)!;
            _entries[id].DeserializerCircular = SerializeCircular<T>;
        }
       return (T?)_entries[id].Item;
    }

    public void DeserializeCircular<T>(IPacker<T> serializer)
    {
        int id = 0;
        foreach(var entry in _entries)
        {
            var position = _initPositions[id++];
            var packerReader = new PackerReader(_buffer[position..], ref this);
            entry.DeserializerCircular(entry.Item, serializer, ref packerReader);
        }
    }

    private static void SerializeCircular<T>(
       object item,
       scoped in object serializer,
       ref PackerReader reader)
    {
        ((IPacker<T>)serializer).DeserializeCircular((T)item, ref reader);
    }
}
