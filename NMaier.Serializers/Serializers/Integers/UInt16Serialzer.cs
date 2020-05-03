using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class UInt16Serialzer : ISerializer<ushort>
  {
    public ushort Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(ushort)];
      stream.BlockingRead(span);
      return BinaryPrimitives.ReadUInt16LittleEndian(span);
#else
      var bytes = new byte[sizeof(ushort)];
      stream.BlockingRead(bytes);
      return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
#endif
    }

    public ushort Deserialize(byte[] bytes, int offset, int length)
    {
      return BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(offset, length));
    }

    public ushort Deserialize(ReadOnlySpan<byte> bytes)
    {
      return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(ushort obj)
    {
      var rv = new byte[sizeof(ushort)];
      BinaryPrimitives.WriteUInt16LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(ushort);
  }
}