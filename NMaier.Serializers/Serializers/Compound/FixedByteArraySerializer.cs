using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  /// <summary>
  ///   Much like the the ByteIdentitySerializer, this serializes byte arrays, but makes sure they have a certain length
  /// </summary>
  [PublicAPI]
  public sealed class FixedByteArraySerializer : ISerializer<byte[]>
  {
    public FixedByteArraySerializer(ushort size)
    {
      Size = size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [NotNull]
    public byte[] Deserialize(Stream stream)
    {
      return stream.BlockingRead(new byte[Size]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Deserialize(byte[] bytes, int offset, int length)
    {
      if (length != Size) {
        ThrowHelpers.ThrowArgumentException("Invalid length", nameof(bytes));
      }

      return bytes.AsSpan(offset, length).ToArray();
    }

    public byte[] Deserialize(ReadOnlySpan<byte> bytes)
    {
      if (bytes.Length != Size) {
        ThrowHelpers.ThrowArgumentException("Invalid length", nameof(bytes));
      }

      return bytes.ToArray();
    }

    public ushort Overhead => 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Serialize(byte[] obj)
    {
      if (obj == null) {
        ThrowHelpers.ThrowArgumentNullException(nameof(obj));
      }

      // ReSharper disable once PossibleNullReferenceException
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      if (obj.Length != Size) {
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(obj));
      }

      return obj.AsSpan().ToArray();
    }

    public ushort Size { get; }
  }
}