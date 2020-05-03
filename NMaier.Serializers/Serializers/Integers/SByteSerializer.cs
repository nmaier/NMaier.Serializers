using System;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class SByteSerializer : ISerializer<sbyte>
  {
    public sbyte Deserialize(Stream stream)
    {
      var rv = stream.ReadByte();
      if (rv < 0) {
        ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(stream));
      }

      return unchecked((sbyte)rv);
    }

    public sbyte Deserialize(byte[] bytes, int offset, int length)
    {
      return unchecked((sbyte)bytes[offset]);
    }

    public sbyte Deserialize(ReadOnlySpan<byte> bytes)
    {
      return unchecked((sbyte)bytes[0]);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(sbyte obj)
    {
      return new[] { unchecked((byte)obj) };
    }

    public ushort Size => sizeof(sbyte);
  }
}