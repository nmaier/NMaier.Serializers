using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class ListSerializer<T> : ISerializer<IList<T>>
  {
    [NotNull]
    private readonly ISerializer<T> underlying;

    public ListSerializer([NotNull] ISerializer<T> underlying)
    {
      this.underlying = underlying;
    }

    public IList<T> Deserialize(Stream stream)
    {
      var len = stream.ReadPackedLength(out _);
      if (len == 0) {
        return new List<T>();
      }

      var rv = new List<T>((int)len);
      var underlyingSize = underlying.Size;
      if (underlyingSize > 0) {
        var buffer = new byte[underlyingSize];
        for (var i = 0; i < len; ++i) {
          rv.Add(underlying.Deserialize(stream.BlockingRead(buffer)));
        }
      }
      else {
        byte[] buffer = null;
        try {
          for (var i = 0; i < len; ++i) {
            var elen = stream.ReadPackedLength(out _);
            if (elen == 0) {
              rv.Add(underlying.Deserialize(new byte[0]));
              continue;
            }

            if (buffer == null || buffer.Length < elen) {
              if (buffer != null) {
                ArrayPool<byte>.Shared.Return(buffer);
              }

              buffer = ArrayPool<byte>.Shared.Rent((int)elen);
            }

            rv.Add(underlying.Deserialize(stream.BlockingRead(buffer, (int)elen)));
          }
        }
        finally {
          if (buffer != null) {
            ArrayPool<byte>.Shared.Return(buffer);
          }
        }
      }

      return rv;
    }

    public IList<T> Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public IList<T> Deserialize(ReadOnlySpan<byte> bytes)
    {
      var len = bytes.ReadPackedLength(out var read);
      if (len == 0) {
        return new List<T>();
      }

      bytes = bytes.Slice(read);
      var rv = new List<T>((int)len);
      var underlyingSize = underlying.Size;
      if (underlyingSize > 0) {
        for (var i = 0; i < len; ++i) {
          rv.Add(underlying.Deserialize(bytes.Slice(0, underlyingSize)));
          bytes = bytes.Slice(underlyingSize);
        }
      }
      else {
        for (var i = 0; i < len; ++i) {
          var elen = bytes.ReadPackedLength(out read);
          if (elen == 0) {
            bytes = bytes.Slice(read);
            rv.Add(underlying.Deserialize(new byte[0]));
            continue;
          }

          rv.Add(underlying.Deserialize(bytes.Slice(read, (int)elen)));
          bytes = bytes.Slice(read + (int)elen);
        }
      }

      return rv;
    }

    public ushort Overhead => 0;

    public byte[] Serialize(IList<T> obj)
    {
      if (obj == null) {
        ThrowHelpers.ThrowArgumentNullException(nameof(obj));
      }

      // ReSharper disable once PossibleNullReferenceException
      if (obj.Count == 0) {
        return new byte[1];
      }

      using var ms = new MemoryStream();
      ms.WritePackedLength(obj.Count);
      var underlyingSize = underlying.Size;
      if (underlyingSize > 0) {
        foreach (var o in obj.Select(e => underlying.Serialize(e))) {
          if (o.Length != underlyingSize) {
            ThrowHelpers.ThrowArgumentOutOfRangeException(nameof(obj));
          }

          ms.Write(o, 0, underlyingSize);
        }
      }
      else {
        foreach (var o in obj.Select(e => underlying.Serialize(e))) {
          ms.WritePackedLength(o.Length);
          if (o.Length > 0) {
            ms.Write(o, 0, o.Length);
          }
        }
      }

      return ms.ToArray();
    }

    public ushort Size => 0;
  }
}