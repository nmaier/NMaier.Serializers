using System;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class BoolSerializer : ISerializer<bool>
  {
    public bool Deserialize(Stream stream)
    {
      var rv = stream.ReadByte();
      if (rv < 0) {
        ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(stream));
      }

      return rv != 0;
    }

    public bool Deserialize(byte[] bytes, int offset, int length)
    {
      return bytes[offset] != 0;
    }

    public bool Deserialize(ReadOnlySpan<byte> bytes)
    {
      return bytes[0] != 0;
    }

    public ushort Overhead => 0;

    public byte[] Serialize(bool obj)
    {
      return new[] { (byte)(obj ? 0x1 : 0x0) };
    }

    public ushort Size => sizeof(byte);
  }
}