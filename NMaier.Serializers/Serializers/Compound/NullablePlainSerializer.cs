using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  /// <inheritdoc />
  /// <summary>
  ///   Optimized serializer for nullable (PoD) types.
  /// </summary>
  /// <remarks>Adds one byte overhead to the total length</remarks>
  [PublicAPI]
  public sealed class NullablePlainSerializer<T> : ISerializer<T?>
    where T : struct
  {
    private readonly ISerializer<T> underlying;

    public NullablePlainSerializer(ISerializer<T> underlying)
    {
      this.underlying = underlying;
    }

    public T? Deserialize(Stream stream)
    {
      return stream.ReadByte() == 0 ? default(T?) : underlying.Deserialize(stream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public T? Deserialize(ReadOnlySpan<byte> bytes)
    {
      return bytes[0] == 0 ? default(T?) : underlying.Deserialize(bytes.Slice(1));
    }

    public ushort Overhead => 1;

    public byte[] Serialize(T? obj)
    {
      if (!obj.HasValue) {
        return new[] { (byte)0 };
      }

      var val = underlying.Serialize(obj.Value);
      var rv = new byte[val.Length + 1];
      rv[0] = 1;
      val.AsSpan().CopyTo(rv.AsSpan(1));
      return rv;
    }

    public ushort Size => (ushort)(underlying.Size > 0 ? underlying.Size + Overhead : 0);
  }
}