using System;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class ByteSerializer : ISerializer<byte>
  {
    public byte Deserialize(Stream stream)
    {
      var rv = stream.ReadByte();
      if (rv < 0) {
        ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(stream));
      }

      return (byte)rv;
    }

    public byte Deserialize(byte[] bytes, int offset, int length)
    {
      return bytes[offset];
    }

    public byte Deserialize(ReadOnlySpan<byte> bytes)
    {
      return bytes[0];
    }

    public ushort Overhead => 0;

    public byte[] Serialize(byte obj)
    {
      return new[] { obj };
    }

    public ushort Size => sizeof(byte);
  }
}