using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  /// <inheritdoc />
  /// <summary>
  ///   Native .NET serializer. This implementation uses BinaryFormatter class to serialize objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [PublicAPI]
  public sealed class NativeSerializer<T> : ISerializer<T>
  {
    private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

    public T Deserialize(Stream stream)
    {
      if (stream.Length == 0) {
        return default;
      }

      return (T)binaryFormatter.Deserialize(stream);
    }

    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public T Deserialize(byte[] bytes, int offset, int length)
    {
      if (length == 0) {
        return default;
      }

      using var ms = new MemoryStream(bytes, offset, length, false);
      return Deserialize(ms);
    }

    public T Deserialize(ReadOnlySpan<byte> bytes)
    {
      if (bytes.Length == 0) {
        return default;
      }

      using var ms = new MemoryStream(bytes.ToArray(), 0, bytes.Length, false);
      return Deserialize(ms);
    }

    // Variable overhead, not our place to determine it
    public ushort Overhead => 0;

    public byte[] Serialize(T obj)
    {
      if (obj == null) {
        return new byte[0];
      }

      using var ms = new MemoryStream();
      binaryFormatter.Serialize(ms, obj);
      ms.Flush();
      return ms.ToArray();
    }

    public ushort Size => 0;
  }
}