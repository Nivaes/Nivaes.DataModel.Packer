using System.Buffers.Binary;
using static Nivaes.DataModel.Packer.PackerReader;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveReader
{
    private delegate void DeserializeCircularDelegate(ref RecursiveReader recursiveReader, object item, in object serializer);

    private struct Entry
    {
        public bool IsCreate;
        public object Item;
        public object Deserializer;
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

    private TReference? Read<T, TReference, TDeserializer>(int id, TDeserializer deserializer)
       where TDeserializer : IDeserializer<TReference>, new()
    {
        if (!_entries[id].IsCreate)
        {
            var position = _initPositions[id];
            var packerReader = new PackerReader(_buffer[position..], ref this);
            var idClassType = packerReader.ReadInt32();
            if (idClassType != deserializer.IdClassType)
            {
                throw new Exception($"No coinciden las clase: {idClassType} != {deserializer.IdClassType}");
            }
            var idObject = packerReader.ReadInt32();
            if (idObject != id)
            {
                throw new Exception($"No coinciden el id objet: {idObject} != {id}");
            }

            _entries[id].IsCreate = true;
            _entries[id].Item = deserializer.Deserialize(ref packerReader)!;
            _entries[id].Deserializer = deserializer;
            _entries[id].DeserializerCircular = SerializeCircular<T>;
        }

        return (TReference?)_entries[id].Item;
    }

    public TReference? Read<T, TReference, TDeserializer>(int id)
        where TDeserializer : IDeserializer<TReference>, new()
    {
        var deserializer = new TDeserializer();

        return Read<T, TReference, TDeserializer>(id, deserializer);
    }

    public TReference? Read<T, TReference, TDeserializer>(int id, int idReference, ReferenceSetterDelegate<T, TReference> setter)
       where TDeserializer : IDeserializer<TReference>, new()
    {
        var deserializer = new TDeserializer();

        var value = Read<T, TReference, TDeserializer>(id, deserializer);

        deserializer.ReferenceSetters?[idReference].Reference = value;

        return value;
    }

    public void DeserializeCircular<T>()
    {
        foreach (var entry in _entries)
        {
            entry.DeserializerCircular(ref this, entry.Item, entry.Deserializer);
        }
    }

    private static void SerializeCircular<T>(
       ref RecursiveReader recursiveReader,
       object item,
       in object deserializer)
    {
        var serializerPacker = (IDeserializer<T>)deserializer;
        if (serializerPacker.ReferenceSetters is not null)
        {
            for (int i = 0; i < serializerPacker.ReferenceSetters.Length; i++)
            {
                ref var referenceSetter = ref serializerPacker.ReferenceSetters[i];
                referenceSetter.Reference = recursiveReader._entries[referenceSetter.IdReference].Item;
            }
        }

        serializerPacker.DeserializeCircular((T)item);
    }
}
