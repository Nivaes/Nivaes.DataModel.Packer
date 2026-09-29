using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Nivaes.DataModel.Packer;

public ref struct PackerReader
{
    private readonly ReadOnlySpan<byte> _spanBuffer;
    private int _position;

    private RecursiveReader _recursiveReader;

    internal PackerReader(ReadOnlySpan<byte> buffer)
    {
        _spanBuffer = buffer;
        //_position = 4; // Los 4 primeras posición indican el tamaño.
        _recursiveReader = new RecursiveReader(buffer);
    }

    internal PackerReader(ReadOnlySpan<byte> buffer, ref RecursiveReader recursiveReader)
    {
        _spanBuffer = buffer;
        //_position = 4; // Los 4 primeras posición indican el tamaño.
        _recursiveReader = recursiveReader;
    }

    //public readonly int Position => _position;

    //public readonly int Remaining =>
    //    _span.Length - _position;

    //internal void ReaderSize()
    //{
    //    _position += 4;
    //}

    //internal void Register<T>(T item)
    //    where T : IPackable<T>
    //{
    //    _recursiveReader.Register(item);
    //}

    //internal void ReadItems()
    //{
    //    _recursiveReader.ReadItems();
    //}

    #region Reads
    private ReadOnlySpan<byte> ReadSpan(int size)
    {
        if ((uint)(_position + size) > (uint)_spanBuffer.Length)
            throw new InvalidOperationException(
                "Unexpected end of buffer.");

        var result = _spanBuffer.Slice(_position, size);

        _position += size;

        return result;
    }

    public byte ReadByte()
    {
        return ReadSpan(1)[0];
    }

    public bool ReadBoolean()
    {
        return ReadByte() != 0;
    }

    public short ReadInt16()
    {
        var span = ReadSpan(2);

        return BinaryPrimitives.ReadInt16LittleEndian(span);
    }

    public ushort ReadUInt16()
    {
        var span = ReadSpan(2);

        return BinaryPrimitives.ReadUInt16LittleEndian(span);
    }

    public int ReadInt32()
    {
        var span = ReadSpan(4);

        return BinaryPrimitives.ReadInt32LittleEndian(span);
    }

    public uint ReadUInt32()
    {
        var span = ReadSpan(4);

        return BinaryPrimitives.ReadUInt32LittleEndian(span);
    }

    public long ReadInt64()
    {
        var span = ReadSpan(8);

        return BinaryPrimitives.ReadInt64LittleEndian(span);
    }

    public ulong ReadUInt64()
    {
        var span = ReadSpan(8);

        return BinaryPrimitives.ReadUInt64LittleEndian(span);
    }

    public float ReadSingle()
    {
        return BitConverter.Int32BitsToSingle(ReadInt32());
    }

    public double ReadDouble()
    {
        return BitConverter.Int64BitsToDouble(ReadInt64());
    }

    public decimal ReadDecimal()
    {
        Span<int> bits = stackalloc int[4];

        _spanBuffer
            .Slice(_position, 16)
            .CopyTo(MemoryMarshal.AsBytes(bits));

        _position += 16;

        return new decimal(bits);
    }

    public Guid ReadGuid()
    {
        var span = ReadSpan(16);

        return new Guid(span);
    }

    public DateTime ReadDateTime()
    {
        long ticks = ReadInt64();
        var kind = (DateTimeKind)ReadByte();

        return new DateTime(ticks, kind);
    }

    public DateTimeOffset ReadDateTimeOffset()
    {
        long ticks = ReadInt64();
        short offsetMinutes = ReadInt16();

        return new DateTimeOffset(
            ticks,
            TimeSpan.FromMinutes(offsetMinutes));
    }

    public string ReadString()
    {
        int byteCount = ReadInt32();

        if (byteCount == -1)
            return null!;
        if (byteCount < -1)
            return string.Empty;

        var span = ReadSpan(byteCount);

        return Encoding.UTF8.GetString(span);
    }

    public byte[]? ReadBytes()
    {
        int length = ReadInt32();

        if (length < 0)
            return null;

        var result = GC.AllocateUninitializedArray<byte>(length);

        ReadSpan(length).CopyTo(result);

        return result;
    }

    public ReadOnlySpan<byte> ReadBytesSpan()
    {
        int length = ReadInt32();

        if (length < 0)
            return ReadOnlySpan<byte>.Empty;

        return ReadSpan(length);
    }

    public T? Read<T>()
        where T : IPackable<T>
    {
        int id = ReadInt32();

        if (id == -1)
            return default(T);

        return _recursiveReader.Read<T>(id/*, ref this*/);
    }
    #endregion
}
