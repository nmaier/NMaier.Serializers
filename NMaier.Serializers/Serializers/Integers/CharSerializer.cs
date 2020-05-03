using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class CharSerializer : ISerializer<char>
  {
    public char Deserialize(Stream stream)
    {
#if NETSTANDARD2_1
      Span<byte> span = stackalloc byte[sizeof(char)];
      stream.BlockingRead(span);
      return (char)BinaryPrimitives.ReadUInt16LittleEndian(span);
#else
      var bytes = new byte[sizeof(char)];
      stream.BlockingRead(bytes);
      return (char)BinaryPrimitives.ReadUInt16LittleEndian(bytes);
#endif
    }

    public char Deserialize(byte[] bytes, int offset, int length)
    {
      return (char)BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(offset, length));
    }

    public char Deserialize(ReadOnlySpan<byte> bytes)
    {
      return (char)BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(char obj)
    {
      var rv = new byte[sizeof(char)];
      BinaryPrimitives.WriteUInt16LittleEndian(rv, obj);
      return rv;
    }

    public ushort Size => sizeof(char);
  }
}