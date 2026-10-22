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

    private TReference? Read<TReference, TReferenceDeserializer>(int idObject, TReferenceDeserializer deserializer)
       where TReferenceDeserializer : IDeserializer<TReference>, new()
    {
        if (!_entries[idObject].IsCreate)
        {
            var position = _initPositions[idObject];
            var packerReader = new PackerReader(_buffer[position..], ref this);
            var idClassType = packerReader.ReadInt32();
            if (idClassType != deserializer.IdClassType)
            {
                throw new Exception($"No coinciden las clase: {idClassType} != {deserializer.IdClassType}");
            }
            var idObjectPacket = packerReader.ReadInt32();
            if (idObjectPacket != idObject)
            {
                throw new Exception($"No coinciden el id objet: {idObjectPacket} != {idObject}");
            }

            _entries[idObject].IsCreate = true;
            _entries[idObject].Item = deserializer.Deserialize(ref packerReader)!;
            _entries[idObject].Deserializer = deserializer;
            _entries[idObject].DeserializerCircular = SerializeCircular;
        }

        return (TReference?)_entries[idObject].Item;
    }

    public TReference? Read<TReference, TDeserializer>(int idObject)
        where TDeserializer : IDeserializer<TReference>, new()
    {
        var deserializer = new TDeserializer();

        return Read<TReference, TDeserializer>(idObject, deserializer);
    }

    public TReference? Read<TReference, TReferenceDeserializer>(int idObject, int idReference, ReferenceSetterDelegate<TReference> setter)
       where TReferenceDeserializer : IDeserializer<TReference>, new()
    {
        var deserializer = new TReferenceDeserializer();

        var value = Read<TReference, TReferenceDeserializer>(idObject, deserializer);

        deserializer.ReferenceSetters?[idReference-1].Reference = value;

        return value;
    }

    public void DeserializeCircular<T>()
    {
        foreach (var entry in _entries)
        {
            entry.DeserializerCircular(ref this, entry.Item, entry.Deserializer);
        }
    }

    private static void SerializeCircular(
       ref RecursiveReader recursiveReader,
       object item,
       in object deserializer)
    {
        var serializerPacker = (IDeserializer)deserializer;
        if (serializerPacker.ReferenceSetters is not null)
        {
            for (int i = 0; i < serializerPacker.ReferenceSetters.Length; i++)
            {
                ref var referenceSetter = ref serializerPacker.ReferenceSetters[i];
                referenceSetter.Reference = recursiveReader._entries[i].Item;
            }
        }

        serializerPacker.DeserializeCircular(item);
    }
}
