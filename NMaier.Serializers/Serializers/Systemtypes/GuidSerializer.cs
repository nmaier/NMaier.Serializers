using System;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class GuidSerializer : ISerializer<Guid>
  {
    public Guid Deserialize(Stream stream)
    {
      return new Guid(stream.BlockingRead(new byte[16]));
    }

    public Guid Deserialize(byte[] bytes, int offset, int length)
    {
      return new Guid(bytes.AsSpan(offset, length).ToArray());
    }

    public Guid Deserialize(ReadOnlySpan<byte> bytes)
    {
      return new Guid(bytes.ToArray());
    }

    public ushort Overhead => 0;

    public byte[] Serialize(Guid obj)
    {
      return obj.ToByteArray();
    }

    public ushort Size => 16;
  }
}