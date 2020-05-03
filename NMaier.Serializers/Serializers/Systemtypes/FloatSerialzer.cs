using System;
using System.Buffers.Binary;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class FloatSerialzer : ISerializer<float>
  {
    public float Deserialize(Stream stream)
    {
      byte[] bytes = new byte[4];
      stream.BlockingRead(bytes);
      if (!BitConverter.IsLittleEndian) {
        BinaryPrimitives.WriteInt32BigEndian(bytes, BinaryPrimitives.ReadInt32LittleEndian(bytes));
      }

      return BitConverter.ToSingle(bytes, 0);
    }

    public float Deserialize(byte[] bytes, int offset, int length)
    {
      if (!BitConverter.IsLittleEndian) {
        BinaryPrimitives.WriteInt32BigEndian(bytes, BinaryPrimitives.ReadInt32LittleEndian(bytes));
      }

      return BitConverter.ToSingle(bytes, 0);
    }

    public float Deserialize(ReadOnlySpan<byte> bytes)
    {
      return Deserialize(bytes.ToArray(), 0, bytes.Length);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(float obj)
    {
      if (BitConverter.IsLittleEndian) {
        return BitConverter.GetBytes(obj);
      }

      byte[] rv = BitConverter.GetBytes(obj);
      BinaryPrimitives.WriteInt32LittleEndian(rv, BinaryPrimitives.ReadInt32BigEndian(rv));
      return rv;
    }

    public ushort Size => 4;
  }
}