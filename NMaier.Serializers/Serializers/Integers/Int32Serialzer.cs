using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class Int32Serialzer : ISerializer<int>
  {
    public int Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(int)];
      stream.BlockingRead(span);
      return BinaryPrimitives.ReadInt32LittleEndian(span);
#else
      var bytes = new byte[sizeof(int)];
      stream.BlockingRead(bytes);
      return BinaryPrimitives.ReadInt32LittleEndian(bytes);
#endif
    }

    public int Deserialize(byte[] bytes, int offset, int length)
    {
      return BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(offset, length));
    }

    public int Deserialize(ReadOnlySpan<byte> bytes)
    {
      return BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(int obj)
    {
      var rv = new byte[sizeof(int)];
      BinaryPrimitives.WriteInt32LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(int);
  }
}