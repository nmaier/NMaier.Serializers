using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  public static class Extensions
  {
    [PublicAPI]
    public static long ReadPackedLength(this Stream stream, out int read)
    {
      Span<byte> bytes = stackalloc byte[8];
      var first = stream.ReadByte();
      if (first < 0) {
        throw new EndOfStreamException();
      }

      bytes[0] = (byte)first;
      var b0 = first & 192;
      switch (b0) {
        case 192:
          read = 8;
#if NETSTANDARD2_1
          stream.BlockingRead(bytes.Slice(1, 7));
#else
        stream.BlockingReadSlow(bytes.Slice(1, 7));
#endif
          return unchecked((long)(BinaryPrimitives.ReadUInt64BigEndian(bytes) ^ 0xC000000000000000UL));
        case 128:
          read = 4;
#if NETSTANDARD2_1
          stream.BlockingRead(bytes.Slice(1, 3));
#else
        stream.BlockingReadSlow(bytes.Slice(1, 3));
#endif
          return (long)BinaryPrimitives.ReadUInt32BigEndian(bytes) ^ 0x80000000U;
        case 64:
          read = 2;
#if NETSTANDARD2_1
          stream.BlockingRead(bytes.Slice(1, 1));
#else
        stream.BlockingReadSlow(bytes.Slice(1, 1));
#endif
          return (long)BinaryPrimitives.ReadUInt16BigEndian(bytes) ^ 0x4000;
        default:
          read = 1;
          return first;
      }
    }

    [PublicAPI]
    public static long ReadPackedLength(this ReadOnlySpan<byte> bytes, out int read)
    {
      var b0 = bytes[0] & 192;
      switch (b0) {
        case 192:
          read = 8;
          return unchecked((long)(BinaryPrimitives.ReadUInt64BigEndian(bytes) ^ 0xC000000000000000UL));
        case 128:
          read = 4;
          return (long)BinaryPrimitives.ReadUInt32BigEndian(bytes) ^ 0x80000000U;
        case 64:
          read = 2;
          return (long)BinaryPrimitives.ReadUInt16BigEndian(bytes) ^ 0x4000;
        default:
          read = 1;
          return bytes[0];
      }
    }

    [PublicAPI]
    public static long ReadPackedLength(this Span<byte> bytes, out int read)
    {
      var b0 = bytes[0] & 192;
      switch (b0) {
        case 192:
          read = 8;
          return unchecked((long)(BinaryPrimitives.ReadUInt64BigEndian(bytes) ^ 0xC000000000000000UL));
        case 128:
          read = 4;
          return (long)BinaryPrimitives.ReadUInt32BigEndian(bytes) ^ 0x80000000U;
        case 64:
          read = 2;
          return (long)BinaryPrimitives.ReadUInt16BigEndian(bytes) ^ 0x4000;
        default:
          read = 1;
          return bytes[0];
      }
    }

    [PublicAPI]
    public static int WritePackedLength(this Span<byte> bytes, long len)
    {
      if (len < 0 || (ulong)len >= 0xC000000000000000UL) {
        ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(len));
      }

      if (len > 0x3FFFFFFF) {
        BinaryPrimitives.WriteUInt64BigEndian(bytes, unchecked((ulong)len | 0xC000000000000000UL));
        return sizeof(ulong);
      }

      if (len > 0x3FFF) {
        BinaryPrimitives.WriteUInt32BigEndian(bytes, unchecked((uint)len | 0x80000000U));
        return sizeof(uint);
      }

      if (len > 0x3F) {
        BinaryPrimitives.WriteUInt16BigEndian(bytes, unchecked((ushort)(len | 0x4000)));
        return sizeof(ushort);
      }

      bytes[0] = unchecked((byte)len);
      return sizeof(byte);
    }

    [PublicAPI]
    public static int WritePackedLength(this Stream stream, long len)
    {
      if (len < 0 || (ulong)len >= 0xC000000000000000UL) {
        ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(len));
      }

      Span<byte> lenBuf = stackalloc byte[sizeof(long)];
      lenBuf = lenBuf.Slice(0, WritePackedLength(lenBuf, len));
#if NETSTANDARD2_1
      stream.Write(lenBuf);
#else
      foreach (var b in lenBuf) {
        stream.WriteByte(b);
      }
#endif
      return lenBuf.Length;
    }

    [PublicAPI]
    public static bool LookupSerializer<T>(out ISerializer<T>? serializer)
    {
      if (!LookupSerializer(typeof(T), out var oserializer)) {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        serializer = default;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
      }

      if (oserializer == null) {
        serializer = null;
      }
      else {
        serializer = (ISerializer<T>)oserializer;
      }

      return true;
    }

    [PublicAPI]
    public static bool LookupSerializer(this Type type, out object? serializer)
    {
      var target = typeof(ISerializer<>).MakeGenericType(type);
      if (Builtins.Serializers.TryGetValue(type, out var candidate) && target.IsInstanceOfType(candidate)) {
        serializer = candidate;
        return true;
      }

      var underlying = Nullable.GetUnderlyingType(type);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
      if (underlying != null && LookupSerializer(underlying, out candidate)) {
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        var serializerType = typeof(NullablePlainSerializer<>).MakeGenericType(underlying);
        serializer = Activator.CreateInstance(serializerType, candidate);
        return true;
      }

      if (type.HasElementType && typeof(Array).IsAssignableFrom(type)) {
        var etype = type.GetElementType();
        if (etype != null && Builtins.Serializers.TryGetValue(etype, out candidate)) {
          var serializerType = typeof(ArraySerializer<>).MakeGenericType(etype);
          serializer = Activator.CreateInstance(serializerType, candidate);
          return true;
        }
      }

      if (type.IsGenericType &&
          typeof(IList<>).MakeGenericType(type.GetGenericArguments().First()).IsAssignableFrom(type)) {
        var etype = type.GetGenericArguments().First();
        if (etype != null && Builtins.Serializers.TryGetValue(etype, out candidate)) {
          var serializerType = typeof(ListSerializer<>).MakeGenericType(etype);
          serializer = Activator.CreateInstance(serializerType, candidate);
          return true;
        }
      }

      serializer = null;
      return false;
    }

    [PublicAPI]
    public static T Deserialize<T>([NotNull] this ISerializer<T> s, [NotNull] byte[] bytes)
    {
      return s.Deserialize(bytes, 0, bytes.Length);
    }

    [PublicAPI]
    public static T Deserialize<T>([NotNull] this ISerializer<T> s, [NotNull] byte[] bytes, int length)
    {
      return s.Deserialize(bytes, 0, length);
    }

    [PublicAPI]
    public static T Deserialize<T>([NotNull] this ISerializer<T> s, ArraySegment<byte> bytes)
    {
      if (bytes.Array == null) {
        ThrowHelpers.ThrowArgumentNullException(nameof(bytes));
#pragma warning disable CS8603 // Possible null reference return.
        return default; // never reached
#pragma warning restore CS8603 // Possible null reference return.
      }

      return s.Deserialize(bytes.Array, bytes.Offset, bytes.Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] BlockingRead([NotNull] this Stream stream, [NotNull] byte[] buffer)
    {
      return BlockingRead(stream, buffer, buffer.Length);
    }

#if NETSTANDARD2_1
    internal static void BlockingRead(this Stream stream, Span<byte> buffer)
    {
      var read = 0;
      while (read < buffer.Length) {
        var increment = stream.Read(buffer.Slice(read));
        if (increment == 0) {
          throw new EndOfStreamException();
        }

        read += increment;
      }
    }
#else
    [UsedImplicitly]
    internal static void BlockingReadSlow(this Stream stream, Span<byte> buffer)
    {
      for (var read
 = 0; read < buffer.Length; ++read) {
        var b
 = stream.ReadByte();
        if (b < 0) {
          throw new EndOfStreamException();
        }

        buffer[read]
 = (byte)b;
      }
    }
#endif

    internal static byte[] BlockingRead([NotNull] this Stream stream, [NotNull] byte[] buffer, int length)
    {
      var read = 0;
      while (read < length) {
        var increment = stream.Read(buffer, read, length - read);
        if (increment == 0) {
          throw new EndOfStreamException();
        }

        read += increment;
      }

      return buffer;
    }
  }
}