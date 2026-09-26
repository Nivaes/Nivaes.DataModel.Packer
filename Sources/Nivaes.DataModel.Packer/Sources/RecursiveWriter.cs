using System.Runtime.CompilerServices;

namespace Nivaes.DataModel.Packer;

internal ref struct RecursiveWriter
{
    private delegate void SerializeDelegate(object item, ref PackerWriter writer);

    private struct Entry
    {
        public int Hash;
        public object Item;
        public SerializeDelegate Serialize;
    }

    private Entry[] _entries;

    private int _count;

    private int _serializeCount;

    public RecursiveWriter()
        :this(5)
    {
    }

    public RecursiveWriter(int capacity = 5)
    {
        _entries = new Entry[capacity];
    }

    public int Add<T>(T item)
        where T : IPackable<T>
    {
        var hash = RuntimeHelpers.GetHashCode(item);
        
        if (_count == _entries.Length)
            Grow();

        int index = _count++;

        _entries[index] = new Entry
        {
            Hash = RuntimeHelpers.GetHashCode(item),
            Item = item!,
            Serialize = Serialize<T>
        };

        return index;
    }

    public bool Exist<T>(T item)
        where T : IPackable<T>
    {
        var hash = RuntimeHelpers.GetHashCode(item);

        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].Hash == hash)
                return true;
        }
        return false;
    }

    public int TryGet(int hash)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_entries[i].Hash == hash)
                return i;
        }

        throw new IndexOutOfRangeException("Item not found");
    }

    private void Grow()
    {
        int newSize = _entries.Length == 0
            ? 4
            : _entries.Length * 2;

        Array.Resize(ref _entries, newSize);
    }

    private static void Serialize<T>(
       object item,
       ref PackerWriter writer)
       where T : IPackable<T>
    {
        ((T)item).Serialize(ref writer);
    }

    public void Serialize(int index, ref PackerWriter writer)
    {
        var entry = _entries[index];
        entry.Serialize(entry.Item, ref writer);
    }

    public void SerializePending(ref PackerWriter writer)
    {
        while (_serializeCount < _count)
        {
            //writer.WriteSize();
            //int end = _count;

            for (int i = _serializeCount; i < _count; i++)
            {
                ref Entry entry = ref _entries[i];

                entry.Serialize(entry.Item, ref writer);

                _serializeCount = i + 1;
                writer.WriteSize();
            }
            writer.RecursiveWriter();
        }
    }
}