using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class UInt32Serialzer : ISerializer<uint>
  {
    public uint Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(uint)];
      stream.BlockingRead(span);
      return BinaryPrimitives.ReadUInt32LittleEndian(span);
#else
      var bytes = new byte[sizeof(uint)];
      stream.BlockingRead(bytes);
      return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
#endif
    }

    public uint Deserialize(byte[] bytes, int offset, int length)
    {
      return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset, length));
    }

    public uint Deserialize(ReadOnlySpan<byte> bytes)
    {
      return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(uint obj)
    {
      var rv = new byte[sizeof(uint)];
      BinaryPrimitives.WriteUInt32LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(uint);
  }
}