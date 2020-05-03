using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class DoubleSerialzer : ISerializer<double>
  {
    public double Deserialize(Stream stream)
    {
      byte[] bytes = new byte[8];
      stream.BlockingRead(bytes);
      if (!BitConverter.IsLittleEndian) {
        BinaryPrimitives.WriteInt64BigEndian(bytes, BinaryPrimitives.ReadInt64LittleEndian(bytes));
      }

      return BitConverter.ToDouble(bytes, 0);
    }

    public double Deserialize(byte[] bytes, int offset, int length)
    {
      if (!BitConverter.IsLittleEndian) {
        BinaryPrimitives.WriteInt64BigEndian(bytes, BinaryPrimitives.ReadInt64LittleEndian(bytes));
      }

      return BitConverter.ToDouble(bytes, 0);
    }

    public double Deserialize(ReadOnlySpan<byte> bytes)
    {
      return Deserialize(bytes.ToArray(), 0, bytes.Length);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(double obj)
    {
      if (BitConverter.IsLittleEndian) {
        return BitConverter.GetBytes(obj);
      }

      byte[] rv = BitConverter.GetBytes(obj);
      BinaryPrimitives.WriteInt64LittleEndian(rv, BinaryPrimitives.ReadInt64BigEndian(rv));
      return rv;
    }

    public ushort Size => 4;
  }
}