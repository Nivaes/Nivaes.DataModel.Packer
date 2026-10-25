using System.Buffers.Binary;
using System.Security.Principal;
using static Nivaes.DataModel.Packer.PackerReader;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveReader
{
    private delegate void DeserializeCircularDelegate(ref RecursiveReader recursiveReader, object item, in object serializer);

    private struct Entry
    {
        public bool IsCreate;
        //public int IdReference;
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
            if (idClassType != TReferenceDeserializer.IdClassType)
            {
                throw new Exception($"No coinciden las clase: {idClassType} != {TReferenceDeserializer.IdClassType}");
            }
            var idObjectPacket = packerReader.ReadInt32();
            if (idObjectPacket != idObject)
            {
                throw new Exception($"No coinciden el id objet: {idObjectPacket} != {idObject}");
            }

            _entries[idObject].IsCreate = true;

            _entries[idObject].Deserializer = deserializer;
            _entries[idObject].DeserializerCircular = DeserializeCircular;
            _entries[idObject].Item = deserializer.Deserialize(ref packerReader)!;
        }

        return (TReference?)_entries[idObject].Item;
    }

    public TReference? Read<TReference, TReferenceDeserializer>(int idObject)
        where TReferenceDeserializer : IDeserializer<TReference>, new()
    {
        var deserializer = new TReferenceDeserializer();
        deserializer.IdObject = idObject;

        var value = Read<TReference, TReferenceDeserializer>(idObject, deserializer);

        if (value != null && deserializer.ReferenceSetters?[numReferences].IdObject != idObject)
        {
            deserializer.ReferenceSetters?[numReferences].IdObject = idObject;
            deserializer.ReferenceSetters?[numReferences].Reference = value;
        }

        numReferences += 1;

        return value;
    }

    private int numReferences = 0;

    public TReference? Read<TReference, TReferenceDeserializer>(int idObject, int idReference)
       where TReferenceDeserializer : IDeserializer<TReference>, new()
    {
        var deserializer = new TReferenceDeserializer();
        deserializer.IdObject = idObject;

        var value = Read<TReference, TReferenceDeserializer>(idObject, deserializer);

        if (value != null)
        {
            deserializer.ReferenceSetters?[numReferences].IdObject = idReference;
            deserializer.ReferenceSetters?[numReferences].Reference = value;
        }

        numReferences += 1;

        return value;
    }

    public void DeserializeCircular()
    {
        foreach (var entry in _entries)
        {
            entry.DeserializerCircular(ref this, entry.Item, entry.Deserializer);
        }
    }

    private static void DeserializeCircular(
       ref RecursiveReader recursiveReader,
       object item,
       in object deserializerObject)
    {
        var deserializer = (IDeserializer)deserializerObject;
        if (deserializer.ReferenceSetters is not null)
        {
            for (int i = 0; i < deserializer.ReferenceSetters.Length; i++)
            {
                ref var referenceSetter = ref deserializer.ReferenceSetters[i];
                var entry = recursiveReader._entries[referenceSetter.IdObject];

                referenceSetter.Reference = entry.Item;

                //foreach (var entry in recursiveReader._entries)
                //{
                //    if (referenceSetter.IdObject == entry.Item)
                //    {
                //        referenceSetter.Reference = entry.Item;
                //        break;
                //    }
                //}
            }
        }

        if (deserializer.IdObject != 0)
        {
            deserializer.DeserializeCircular(item);
        }
    }
}
