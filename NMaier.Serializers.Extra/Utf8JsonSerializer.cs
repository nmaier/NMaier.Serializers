using System;
using System.IO;
using JetBrains.Annotations;
using Utf8Json;

namespace NMaier.Serializers.Extra
{
  [PublicAPI]
  public sealed class Utf8JsonSerializer<T> : ISerializer<T>
  {
    public T Deserialize(Stream stream)
    {
      return JsonSerializer.Deserialize<T>(stream);
    }

    public T Deserialize(byte[] bytes, int offset, int length)
    {
      using var ms = new MemoryStream(bytes, offset, length, false);
      return JsonSerializer.Deserialize<T>(ms);
    }

    public T Deserialize(ReadOnlySpan<byte> bytes)
    {
      using var ms = new MemoryStream(bytes.ToArray(), 0, bytes.Length, false);
      return JsonSerializer.Deserialize<T>(ms);
    }

    public ushort Overhead => 0;

    public byte[] Serialize(T obj)
    {
      return JsonSerializer.Serialize(obj);
    }

    public ushort Size => 0;
  }
}