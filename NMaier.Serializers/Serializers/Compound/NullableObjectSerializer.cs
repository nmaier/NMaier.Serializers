using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  /// <inheritdoc />
  /// <summary>
  ///   Optimized serializer for nullable types.
  ///   This is most useful for use with underlying serializers that still produced a fixed length or at least optimized
  ///   representation.
  /// </summary>
  /// <example>new NullableObjectSerializer&lt;byte[]&gt;(new FixedByteSerializer(32))</example>
  /// <remarks>Adds one byte overhead to the total length</remarks>
  [PublicAPI]
  public sealed class NullableObjectSerializer<T> : ISerializer<T>
  {
    private readonly ISerializer<T> underlying;

    public NullableObjectSerializer(ISerializer<T> underlying)
    {
      this.underlying = underlying;
    }

    public T Deserialize(Stream stream)
    {
      return stream.ReadByte() == 0 ? default : underlying.Deserialize(stream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public T Deserialize(ReadOnlySpan<byte> bytes)
    {
      return bytes[0] == 0 ? default : underlying.Deserialize(bytes.Slice(1));
    }

    public ushort Overhead => 1;

    public byte[] Serialize(T obj)
    {
      if (obj == null) {
        return new[] { (byte)0 };
      }

      var val = underlying.Serialize(obj);
      var rv = new byte[val.Length + 1];
      rv[0] = 1;
      val.AsSpan().CopyTo(rv.AsSpan(1));
      return rv;
    }

    public ushort Size => (ushort)(underlying.Size > 0 ? underlying.Size + Overhead : 0);
  }
}