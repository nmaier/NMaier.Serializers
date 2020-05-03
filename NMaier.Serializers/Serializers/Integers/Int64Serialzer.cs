using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class Int64Serialzer : ISerializer<long>
  {
    public long Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(long)];
      stream.BlockingRead(span);
      return BinaryPrimitives.ReadInt64LittleEndian(span);
#else
      var bytes = new byte[sizeof(long)];
      stream.BlockingRead(bytes);
      return BinaryPrimitives.ReadInt64LittleEndian(bytes);
#endif
    }

    public long Deserialize(byte[] bytes, int offset, int length)
    {
      return BinaryPrimitives.ReadInt64LittleEndian(bytes.AsSpan(offset, length));
    }

    public long Deserialize(ReadOnlySpan<byte> bytes)
    {
      return BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(long obj)
    {
      var rv = new byte[sizeof(long)];
      BinaryPrimitives.WriteInt64LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(long);
  }
}