using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NMaier.Serializers
{
  [JetBrains.Annotations.PublicAPI]
  public sealed class FixedKeyValueSerializer<TKey, TValue> : ISerializer<KeyValuePair<TKey, TValue>>
  {
    private readonly ISerializer<TKey> keySerializer;
    private readonly ushort keySize;
    private readonly ISerializer<TValue> valueSerializer;
    private readonly ushort valueSize;

    public FixedKeyValueSerializer([JetBrains.Annotations.NotNull] ISerializer<TKey> keySerializer,
      [JetBrains.Annotations.NotNull] ISerializer<TValue> valueSerializer)
    {
      this.keySerializer = keySerializer;
      this.valueSerializer = valueSerializer;
      keySize = keySerializer.Size;
      valueSize = valueSerializer.Size;
    }

    public KeyValuePair<TKey, TValue> Deserialize(Stream stream)
    {
      var bytes = new byte[keySize + valueSize];
      stream.BlockingRead(bytes);
      return Deserialize(bytes, 0, keySize + valueSize);
    }

    public KeyValuePair<TKey, TValue> Deserialize(byte[] bytes, int offset, int length)
    {
      return new KeyValuePair<TKey, TValue>(
        keySerializer.Deserialize(bytes, offset, keySize),
        valueSerializer.Deserialize(bytes, offset + keySize, valueSize)
      );
    }

    public KeyValuePair<TKey, TValue> Deserialize(ReadOnlySpan<byte> bytes)
    {
      return new KeyValuePair<TKey, TValue>(
        keySerializer.Deserialize(bytes),
        valueSerializer.Deserialize(bytes.Slice(keySize)));
    }

    public ushort Overhead => 0;

    [SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
    public byte[] Serialize(KeyValuePair<TKey, TValue> obj)
    {
      var key = keySerializer.Serialize(obj.Key);
      var value = valueSerializer.Serialize(obj.Value);
      var rv = new byte[keySize + valueSize];
      var s = rv.AsSpan();
      key.CopyTo(s.Slice(0, keySize));
      value.CopyTo(s.Slice(keySize, valueSize));
      return rv;
    }

    public ushort Size => (ushort)(keySize + valueSize);
  }
}