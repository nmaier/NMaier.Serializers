using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class TimeSpanSerialzer : ISerializer<TimeSpan>
  {
    public TimeSpan Deserialize(Stream stream)
    {
#if NETSTANDARD2_0 || NET48
      var bytes = new byte[8];
#else
      Span<byte> bytes = stackalloc byte[8];
#endif

      stream.BlockingRead(bytes);
      return TimeSpan.FromTicks(BinaryPrimitives.ReadInt64LittleEndian(bytes));
    }

    public TimeSpan Deserialize(byte[] bytes, int offset, int length)
    {
      return TimeSpan.FromTicks(BinaryPrimitives.ReadInt64LittleEndian(bytes.AsSpan(offset, length)));
    }

    public TimeSpan Deserialize(ReadOnlySpan<byte> bytes)
    {
      return TimeSpan.FromTicks(BinaryPrimitives.ReadInt64LittleEndian(bytes));
    }

    public ushort Overhead => 0;

    public byte[] Serialize(TimeSpan obj)
    {
      byte[] rv = new byte[8];
      BinaryPrimitives.WriteInt64LittleEndian(rv, obj.Ticks);
      return rv;
    }

    public ushort Size => 8;
  }
}