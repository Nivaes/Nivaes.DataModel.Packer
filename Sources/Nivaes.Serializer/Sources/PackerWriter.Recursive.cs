using System.Runtime.CompilerServices;

namespace Nivaes.Serializer;

public ref partial struct PackerWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Add<T>(T item, in ISerializer<T> serializer)
    {
        var hash = RuntimeHelpers.GetHashCode(item);

        if (Exist(hash, out var index))
        {
            return index;
        }

        if (_count == _entries.Length)
            Grow();

        index = _count++;

        _entries[index] = new Entry
        {
            Hash = hash,
            Serializer = serializer,
            Item = item!,
            Serialize = Serialize<T>
        };

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Grow()
    {
        int newSize = _entries.Length == 0
            ? 4
            : _entries.Length * 2;

        Array.Resize(ref _entries, newSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Serialize<T>(scoped in object item, scoped in object serializer, ref PackerWriter writer)
    {
        ((ISerializer<T>)serializer).Serialize((T)item, ref writer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> WriteRoot<T>(in T? value, scoped in ISerializer<T> serializer)
    {
        _ = Add<T>(value!, serializer);

        SerializePending();

        return ToSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SerializePending()
    {
        while (_serializeCount < _count)
        {           
            ref Entry entry = ref _entries[_serializeCount++];

            if (entry.Item is not null)
            {
                ReserveSizeInt32();

                entry.Serialize(entry.Item, entry.Serializer, ref this);

                WriteSize();
            }
        }
    }
}