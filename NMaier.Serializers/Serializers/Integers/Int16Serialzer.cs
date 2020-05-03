using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class Int16Serialzer : ISerializer<short>
  {
    public short Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(short)];
      stream.BlockingRead(span);
      return BinaryPrimitives.ReadInt16LittleEndian(span);
#else
      var bytes = new byte[sizeof(short)];
      stream.BlockingRead(bytes);
      return BinaryPrimitives.ReadInt16LittleEndian(bytes);
#endif
    }

    public short Deserialize(byte[] bytes, int offset, int length)
    {
      return BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan(offset, length));
    }

    public short Deserialize(ReadOnlySpan<byte> bytes)
    {
      return BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(short obj)
    {
      var rv = new byte[sizeof(short)];
      BinaryPrimitives.WriteInt16LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(short);
  }
}