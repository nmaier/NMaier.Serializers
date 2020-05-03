using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace NMaier.Serializers
{
  [JetBrains.Annotations.PublicAPI]
  public sealed class VariableKeyValueSerializer<TKey, TValue> : ISerializer<KeyValuePair<TKey, TValue>>
  {
    private readonly ISerializer<TKey> keySerializer;
    private readonly ISerializer<TValue> valueSerializer;

    public VariableKeyValueSerializer([JetBrains.Annotations.NotNull] ISerializer<TKey> keySerializer,
      [JetBrains.Annotations.NotNull] ISerializer<TValue> valueSerializer)
    {
      this.keySerializer = keySerializer;
      this.valueSerializer = valueSerializer;
    }

    public KeyValuePair<TKey, TValue> Deserialize(Stream stream)
    {
      var size = new byte[sizeof(ushort)];
      stream.BlockingRead(size);
      var key = new byte[BinaryPrimitives.ReadUInt16LittleEndian(size)];
      stream.BlockingRead(key);

      stream.BlockingRead(size);
      var value = new byte[BinaryPrimitives.ReadUInt16LittleEndian(size)];
      stream.BlockingRead(value);
      return new KeyValuePair<TKey, TValue>(
        keySerializer.Deserialize(key),
        valueSerializer.Deserialize(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public KeyValuePair<TKey, TValue> Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public KeyValuePair<TKey, TValue> Deserialize(ReadOnlySpan<byte> bytes)
    {
      var size = BinaryPrimitives.ReadUInt16LittleEndian(bytes);
      bytes = bytes.Slice(sizeof(ushort));
      var key = bytes.Slice(0, size);
      bytes = bytes.Slice(size);

      size = BinaryPrimitives.ReadUInt16LittleEndian(bytes);
      bytes = bytes.Slice(sizeof(ushort));
      var value = bytes.Slice(0, size);
      return new KeyValuePair<TKey, TValue>(
        keySerializer.Deserialize(bytes.Slice(sizeof(ushort), key.Length)),
        valueSerializer.Deserialize(bytes.Slice(sizeof(ushort) * 2 + key.Length, value.Length))
      );
    }

    public ushort Overhead => (ushort)(keySerializer.Overhead + valueSerializer.Overhead + sizeof(ushort) * 2);

    [SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
    public byte[] Serialize(KeyValuePair<TKey, TValue> obj)
    {
      var key = keySerializer.Serialize(obj.Key);
      var value = valueSerializer.Serialize(obj.Value);
      var rv = new byte[key.Length + value.Length + sizeof(ushort) * 2];

      var s = rv.AsSpan();
      BinaryPrimitives.WriteUInt16LittleEndian(s, (ushort)key.Length);
      s = s.Slice(sizeof(ushort));
      key.CopyTo(s);
      s = s.Slice(key.Length);

      BinaryPrimitives.WriteUInt16LittleEndian(s, (ushort)value.Length);
      s = s.Slice(sizeof(ushort));
      value.CopyTo(s);
      return rv;
    }

    public ushort Size => 0;
  }
}