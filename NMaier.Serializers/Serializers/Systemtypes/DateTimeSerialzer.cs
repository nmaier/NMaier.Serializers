using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class DateTimeSerialzer : ISerializer<DateTime>
  {
    public DateTime Deserialize(Stream stream)
    {
#if NETSTANDARD2_0 || NET48
      var bytes = new byte[8];
#else
      Span<byte> bytes = stackalloc byte[8];
#endif
      stream.BlockingRead(bytes);
      return DateTime.FromBinary(BinaryPrimitives.ReadInt64LittleEndian(bytes));
    }

    public DateTime Deserialize(byte[] bytes, int offset, int length)
    {
      return DateTime.FromBinary(BinaryPrimitives.ReadInt64LittleEndian(bytes.AsSpan(offset, length)));
    }

    public DateTime Deserialize(ReadOnlySpan<byte> bytes)
    {
      //IL_0000: Unknown result type (might be due to invalid IL or missing references)
      return DateTime.FromBinary(BinaryPrimitives.ReadInt64LittleEndian(bytes));
    }

    public ushort Overhead => 0;

    public byte[] Serialize(DateTime obj)
    {
      //IL_0008: Unknown result type (might be due to invalid IL or missing references)
      byte[] rv = new byte[8];
      BinaryPrimitives.WriteInt64LittleEndian(rv.AsSpan(), obj.ToBinary());
      return rv;
    }

    public ushort Size => 8;
  }
}