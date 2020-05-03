using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class DecimalSerializer : ISerializer<decimal>
  {
    public decimal Deserialize(Stream stream)
    {
#if NETSTANDARD2_0 || NET48
      var bytes = new byte[sizeof(int) * 4];
#else
      Span<byte> bytes = stackalloc byte[sizeof(int) * 4];
#endif
      stream.BlockingRead(bytes);
      return new decimal(MemoryMarshal.Cast<byte, int>(bytes).ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public decimal Deserialize(ReadOnlySpan<byte> bytes)
    {
      int[] bits = {
        BinaryPrimitives.ReadInt32LittleEndian(bytes), BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(4)),
        BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(8)), BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(12))
      };
      return new decimal(bits);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(decimal obj)
    {
      int[] bits = decimal.GetBits(obj);
      if (!BitConverter.IsLittleEndian) {
        bits = bits.Select(BinaryPrimitives.ReverseEndianness).ToArray();
      }

      return MemoryMarshal.AsBytes(bits.AsSpan()).ToArray();
    }

    public ushort Size => 16;
  }
}