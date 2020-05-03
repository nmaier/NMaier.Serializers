using System;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  /// <inheritdoc />
  /// <summary>
  ///   Simple Serializer based on functions
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [PublicAPI]
  public sealed class SimpleSerializer<T> : ISerializer<T>
  {
    private readonly Func<byte[], int, int, T> deserialize;
    private readonly Func<T, byte[]> serialize;

    /// <summary>
    ///   Initializes a new instance of the SimpleSerializer using specified serialization routines.
    /// </summary>
    /// <param name="serialize">Serializer function</param>
    /// <param name="deserialize">Deserializer function</param>
    /// <param name="overhead">Overhead in bytes (see ISerializer)</param>
    /// <param name="size">Size in bytes (see ISerializer)</param>
    public SimpleSerializer([NotNull] Func<T, byte[]> serialize, [NotNull] Func<byte[], int, int, T> deserialize,
      ushort overhead, ushort size)
    {
      this.serialize = serialize ?? throw new ArgumentNullException(nameof(serialize));
      this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
      Overhead = overhead;
      Size = (ushort)(size > 0 ? size + overhead : 0);
    }

    public T Deserialize(Stream stream)
    {
      switch (stream) {
        case MemoryStream ms:
          return deserialize(ms.ToArray(), 0, (int)ms.Length);
        default:
          using (var ms = new MemoryStream()) {
            stream.CopyTo(ms);
            return deserialize(ms.ToArray(), 0, (int)ms.Length);
          }
      }
    }

    public T Deserialize(byte[] bytes, int offset, int length)
    {
      return deserialize(bytes, offset, length);
    }

    public T Deserialize(ReadOnlySpan<byte> bytes)
    {
      return deserialize(bytes.ToArray(), 0, bytes.Length);
    }

    public ushort Overhead { get; }

    public byte[] Serialize(T obj)
    {
      return serialize(obj);
    }

    public ushort Size { get; }
  }
}