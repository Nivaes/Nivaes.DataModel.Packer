using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.DataModel.Packer;

public ref struct PackerWriter
{
    private readonly ArrayBufferWriter<byte> _writer;

    public PackerWriter()
    {
        _writer = new ArrayBufferWriter<byte>(256);
    }

    public PackerWriter(int initialCapacity = 256)
    {
        _writer = new ArrayBufferWriter<byte>(initialCapacity);
    }

    public readonly int Position =>
        _writer.WrittenCount;

    public readonly int Length =>
        _writer.WrittenCount;

    public readonly ReadOnlySpan<byte> WrittenSpan =>
        _writer.WrittenSpan;

    public readonly ReadOnlyMemory<byte> WrittenMemory =>
        _writer.WrittenMemory;

    public readonly byte[] ToArray() =>
        _writer.WrittenSpan.ToArray();

    public void Write(byte value)
    {
        Span<byte> span = _writer.GetSpan(1);

        span[0] = value;

        _writer.Advance(1);
    }

    public void Write(bool value)
    {
        Write(value ? (byte)1 : (byte)0);
    }

    public void Write(short value)
    {
        Span<byte> span = _writer.GetSpan(2);

        BinaryPrimitives.WriteInt16LittleEndian(span, value);

        _writer.Advance(2);
    }

    public void Write(ushort value)
    {
        Span<byte> span = _writer.GetSpan(2);

        BinaryPrimitives.WriteUInt16LittleEndian(span, value);

        _writer.Advance(2);
    }

    public void Write(int value)
    {
        Span<byte> span = _writer.GetSpan(4);

        BinaryPrimitives.WriteInt32LittleEndian(span, value);

        _writer.Advance(4);
    }

    public void Write(uint value)
    {
        Span<byte> span = _writer.GetSpan(4);

        BinaryPrimitives.WriteUInt32LittleEndian(span, value);

        _writer.Advance(4);
    }

    public void Write(long value)
    {
        Span<byte> span = _writer.GetSpan(8);

        BinaryPrimitives.WriteInt64LittleEndian(span, value);

        _writer.Advance(8);
    }

    public void Write(ulong value)
    {
        Span<byte> span = _writer.GetSpan(8);

        BinaryPrimitives.WriteUInt64LittleEndian(span, value);

        _writer.Advance(8);
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
        Span<byte> span = _writer.GetSpan(16);

        value.TryWriteBytes(span);

        _writer.Advance(16);
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

        int byteCount = Encoding.UTF8.GetByteCount(value);

        Write(byteCount);

        Span<byte> span = _writer.GetSpan(byteCount);

        int written = Encoding.UTF8.GetBytes(value, span);

        _writer.Advance(written);
    }

    public void Write(byte[]? value)
    {
        if (value is null)
        {
            Write(-1);
            return;
        }

        Write(value.AsSpan());
    }

    public void Write(ReadOnlySpan<byte> value)
    {
        Write(value.Length);

        if (value.IsEmpty)
            return;

        Span<byte> span = _writer.GetSpan(value.Length);

        value.CopyTo(span);

        _writer.Advance(value.Length);
    }
}
