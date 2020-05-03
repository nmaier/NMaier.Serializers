using System;
using NUnit.Framework;

namespace NMaier.Serializers.Tests
{
  public sealed class Basic
  {
    [Test]
    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(byte.MaxValue - 4)]
    [TestCase(byte.MaxValue - 3)]
    [TestCase(byte.MaxValue - 2)]
    [TestCase(byte.MaxValue - 1)]
    [TestCase(byte.MaxValue)]
    [TestCase(byte.MaxValue + 1)]
    [TestCase(byte.MaxValue + 2)]
    [TestCase(byte.MaxValue + 3)]
    [TestCase(byte.MaxValue + 4)]
    [TestCase(ushort.MaxValue - 4)]
    [TestCase(ushort.MaxValue - 3)]
    [TestCase(ushort.MaxValue - 2)]
    [TestCase(ushort.MaxValue - 1)]
    [TestCase(ushort.MaxValue)]
    [TestCase(ushort.MaxValue + 1)]
    [TestCase(ushort.MaxValue + 2)]
    [TestCase(ushort.MaxValue + 3)]
    [TestCase(ushort.MaxValue + 4)]
    [TestCase(uint.MaxValue - 4)]
    [TestCase(uint.MaxValue - 3)]
    [TestCase(uint.MaxValue - 2)]
    [TestCase(uint.MaxValue - 1)]
    [TestCase(uint.MaxValue)]
    [TestCase((long)uint.MaxValue + 1)]
    [TestCase((long)uint.MaxValue + 2)]
    [TestCase((long)uint.MaxValue + 3)]
    [TestCase((long)uint.MaxValue + 4)]
    public void TestPackedLength(long len)
    {
      var buf = new byte[16];
      buf.AsSpan().WritePackedLength(len);
      Assert.AreEqual(buf.AsSpan().ReadPackedLength(out _), len);
    }
  }
}