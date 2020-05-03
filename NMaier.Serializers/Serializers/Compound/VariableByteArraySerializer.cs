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
  public sealed class VariableByteArraySerializer : ISerializer<byte[]>
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [NotNull]
    public byte[] Deserialize(Stream stream)
    {
      var len = stream.ReadPackedLength(out _);
      return len == 0 ? Array.Empty<byte>() : stream.BlockingRead(new byte[len]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public byte[] Deserialize(ReadOnlySpan<byte> bytes)
    {
      var len = bytes.ReadPackedLength(out var read);
      return len == 0 ? Array.Empty<byte>() : bytes.Slice(read, (int)len).ToArray();
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
      if (obj.Length == 0) {
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return new byte[sizeof(ushort)];
      }

      using var ms = new MemoryStream();
      ms.WritePackedLength(obj.Length);
      ms.Write(obj, 0, obj.Length);
      return ms.ToArray();
    }

    public ushort Size => 0;
  }
}