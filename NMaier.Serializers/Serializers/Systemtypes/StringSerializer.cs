using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  [PublicAPI]
  public sealed class StringSerializer : ISerializer<string>
  {
    private readonly Encoding encoding;

    public StringSerializer()
      : this(Encoding.UTF8)
    {
    }

    public StringSerializer(Encoding encoding)
    {
      this.encoding = encoding;
    }

    public string Deserialize(Stream stream)
    {
      long len = stream.ReadPackedLength(out _);
      using ConstrainedStream cs = new ConstrainedStream(stream, (int)len);
      using StreamReader r = new StreamReader(cs, encoding);
      return r.ReadToEnd();
    }

    public string Deserialize(byte[] bytes, int offset, int length)
    {
      return Deserialize(bytes.AsSpan(offset, length));
    }

    public string Deserialize(ReadOnlySpan<byte> bytes)
    {
      long len = bytes.ReadPackedLength(out var read);
#if NETSTANDARD2_0 || NET48
      return len == 0L ? string.Empty : encoding.GetString(bytes.Slice(read, (int)len).ToArray());
#else
      return len == 0L ? string.Empty : encoding.GetString(bytes.Slice(read, (int)len));
#endif
    }

    public ushort Overhead => 0;

    public byte[] Serialize(string obj)
    {
      if (obj == null) {
        ThrowHelpers.ThrowArgumentNullException("obj");
      }

      if (string.IsNullOrEmpty(obj)) {
        return new byte[1];
      }

      byte[] bytes = encoding.GetBytes(obj);
      Span<byte> packed2 = stackalloc byte[8];
      packed2 = packed2.Slice(0, packed2.WritePackedLength(bytes.Length));
      byte[] rv = new byte[bytes.Length + packed2.Length];
      packed2.CopyTo(rv);
      bytes.AsSpan().CopyTo(rv.AsSpan(packed2.Length));
      return rv;
    }

    public ushort Size => 0;

    private sealed class ConstrainedStream : Stream
    {
      private readonly int maxLen;
      private readonly Stream wrapped;

      private int cur;

      internal ConstrainedStream(Stream wrapped, int maxLen)
      {
        this.wrapped = wrapped;
        this.maxLen = maxLen;
      }

      public override bool CanRead => true;

      public override bool CanSeek => false;

      public override bool CanWrite => false;

      public override long Length
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public override long Position
      {
        get
        {
          throw new NotSupportedException();
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      public override void Flush()
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        int read2 = Math.Min(count, maxLen - cur);
        if (read2 == 0) {
          return 0;
        }

        read2 = wrapped.Read(buffer, offset, read2);
        if (read2 > 0) {
          cur += read2;
        }

        return read2;
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        throw new NotSupportedException();
      }

      public override void SetLength(long value)
      {
        throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }
    }
  }
}