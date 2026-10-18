using System.Runtime.CompilerServices;

namespace Nivaes.DataModel.Packer;

public ref partial struct PackerWriter
{
    public void Add<T>(T item, in IPacker<T> serializer, int idReference)
    {
        var hash = RuntimeHelpers.GetHashCode(item);

        //if(Exist(hash, out var index))
        //{
        //    return index;
        //}
        
        if (_count++ == _entries.Length)
            Grow();

        //index = _count++;

        _entries[idReference] = new Entry
        {
            Hash = hash,
            IdClass = serializer.IdClassType,
            Serializer = serializer,
            Item = item!,
            Serialize = Serialize<T>
        };

        //return index;
    }

    public bool Exist(int hash, out int index)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].Hash == hash)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    private void Grow()
    {
        int newSize = _entries.Length == 0
            ? 4
            : _entries.Length * 2;

        Array.Resize(ref _entries, newSize);
    }

    private static void Serialize<T>(scoped in object item, scoped in object serializer, ref PackerWriter writer)
    {
        ((IPacker<T>)serializer).Serialize((T)item, ref writer);
    }

    public ReadOnlySpan<byte> WriteRoot<T>(in T? value, scoped in IPacker<T> serializer)
    {
        /*var id =*/ Add<T>(value!, serializer, 0);

        SerializePending();

        return ToSpan();
    }

    private void SerializePending()
    {
        while (_serializeCount < _count)
        {
            for (; _serializeCount < _count; _serializeCount++)
            {
                ref Entry entry = ref _entries[_serializeCount];

                ReserveSizeInt32();
                Write(entry.IdClass);
                Write(_serializeCount);
                
                entry.Serialize(entry.Item, entry.Serializer, ref this);

                WriteSize();
            }
            //writer.RecursiveWriter();
            SerializePending();
        }
    }
}