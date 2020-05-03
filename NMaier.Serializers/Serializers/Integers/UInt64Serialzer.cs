using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class UInt64Serialzer : ISerializer<ulong>
  {
    public ulong Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(ulong)];
      stream.BlockingRead(span);
      return BinaryPrimitives.ReadUInt64LittleEndian(span);
#else
      var bytes = new byte[sizeof(ulong)];
      stream.BlockingRead(bytes);
      return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
#endif
    }

    public ulong Deserialize(byte[] bytes, int offset, int length)
    {
      return BinaryPrimitives.ReadUInt64LittleEndian(bytes.AsSpan(offset, length));
    }

    public ulong Deserialize(ReadOnlySpan<byte> bytes)
    {
      return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(ulong obj)
    {
      var rv = new byte[sizeof(ulong)];
      BinaryPrimitives.WriteUInt64LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(ulong);
  }
}