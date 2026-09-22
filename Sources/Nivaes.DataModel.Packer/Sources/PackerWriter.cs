using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer;

public ref struct PackerWriter
{
    private byte[] _buffer;
    private Span<byte> _span;
    private int _position;

    public PackerWriter(int initialCapacity = 256)
    {
        _buffer = GC.AllocateUninitializedArray<byte>(initialCapacity);
        _span = _buffer;
        _position = 0;
    }

    public readonly int Position => _position;

    public readonly int Length => _position;

    public readonly ReadOnlySpan<byte> WrittenSpan =>
        _span[.._position];

    public readonly byte[] ToArray()
    {
        return _span[.._position].ToArray();
    }

    private void Ensure(int size)
    {
        if ((uint)(_position + size) <= (uint)_span.Length)
            return;

        Grow(size);
    }

    private void Grow(int size)
    {
        int required = _position + size;

        int newSize = Math.Max(
            required,
            Math.Max(_span.Length * 2, 256));

        var newBuffer =
            GC.AllocateUninitializedArray<byte>(newSize);

        _span[.._position].CopyTo(newBuffer);

        _buffer = newBuffer;
        _span = newBuffer;
    }

    public void Write(byte value)
    {
        Ensure(1);

        _span[_position++] = value;
    }

    public void Write(bool value)
    {
        Write(value ? (byte)1 : (byte)0);
    }

    public void Write(short value)
    {
        Ensure(2);

        BinaryPrimitives.WriteInt16LittleEndian(
            _span[_position..],
            value);

        _position += 2;
    }

    public void Write(ushort value)
    {
        Ensure(2);

        BinaryPrimitives.WriteUInt16LittleEndian(
            _span[_position..],
            value);

        _position += 2;
    }

    public void Write(int value)
    {
        Ensure(4);

        BinaryPrimitives.WriteInt32LittleEndian(
            _span[_position..],
            value);

        _position += 4;
    }

    public void Write(uint value)
    {
        Ensure(4);

        BinaryPrimitives.WriteUInt32LittleEndian(
            _span[_position..],
            value);

        _position += 4;
    }

    public void Write(long value)
    {
        Ensure(8);

        BinaryPrimitives.WriteInt64LittleEndian(
            _span[_position..],
            value);

        _position += 8;
    }

    public void Write(ulong value)
    {
        Ensure(8);

        BinaryPrimitives.WriteUInt64LittleEndian(
            _span[_position..],
            value);

        _position += 8;
    }

    public void Write(float value)
    {
        Write(BitConverter.SingleToInt32Bits(value));
    }

    public void Write(double value)
    {
        Write(BitConverter.DoubleToInt64Bits(value));
    }

    public void Write(decimal value)
    {
        int[] bits = decimal.GetBits(value);

        Write(bits[0]);
        Write(bits[1]);
        Write(bits[2]);
        Write(bits[3]);
    }

    public void Write(Guid value)
    {
        Ensure(16);

        value.TryWriteBytes(_span[_position..]);

        _position += 16;
    }

    public void Write(DateTime value)
    {
        Write(value.Ticks);
        Write((byte)value.Kind);
    }

    public void Write(DateTimeOffset value)
    {
        Write(value.Ticks);
        Write((short)value.Offset.TotalMinutes);
    }

    public void Write(string? value)
    {
        if (value is null)
        {
            Write(-1);
            return;
        }
        if(string.IsNullOrEmpty(value))
        {
            Write(-2);
            return;
        }

        int byteCount = Encoding.UTF8.GetByteCount(value);

        Write(byteCount);

        Ensure(byteCount);

        int written = Encoding.UTF8.GetBytes(
            value,
            _span[_position..]);

        _position += written;
    }

    public void Write(byte[]? value)
    {
        if (value is null)
        {
            Write(-1);
            return;
        }

        Write(value.Length);

        Ensure(value.Length);

        value.AsSpan().CopyTo(_span[_position..]);

        _position += value.Length;
    }

    public void Write(ReadOnlySpan<byte> value)
    {
        Write(value.Length);

        Ensure(value.Length);

        value.CopyTo(_span[_position..]);

        _position += value.Length;
    }
}
