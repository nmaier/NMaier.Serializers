using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NMaier.Serializers.Tests
{
  public sealed class Compound
  {
    private static void TestArrayEqual<T>(ISerializer<T[]> serializer, T[] val) where T : IEquatable<T>
    {
      var s = serializer.Serialize(val);
      Assert.True(serializer.Deserialize(s.AsSpan()).AsSpan().SequenceEqual(val.AsSpan()));
      Assert.True(serializer.Deserialize(s, 0, s.Length).AsSpan().SequenceEqual(val.AsSpan()));
      using var ms = new MemoryStream(s, 0, s.Length, false);
      Assert.True(serializer.Deserialize(ms).AsSpan().SequenceEqual(val.AsSpan()));
    }

    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    private static void TestListEqual<T>(ISerializer<IList<T>> serializer, T[] val) where T : IEquatable<T>
    {
      var s = serializer.Serialize(val.ToList());
      Assert.True(serializer.Deserialize(s.AsSpan()).ToArray().AsSpan().SequenceEqual(val.AsSpan()));
      Assert.True(serializer.Deserialize(s, 0, s.Length).ToArray().AsSpan().SequenceEqual(val.AsSpan()));
      using var ms = new MemoryStream(s, 0, s.Length, false);
      Assert.True(serializer.Deserialize(ms).ToArray().AsSpan().SequenceEqual(val.AsSpan()));
    }

    [Test]
    public void TestFixedByteArraySerializer()
    {
      var serializer = new FixedByteArraySerializer(10);
      TestArrayEqual(
        serializer,
        new byte[] { 1, 100, 128, byte.MaxValue, byte.MinValue, 1, 100, 128, byte.MaxValue, byte.MinValue });
      TestArrayEqual(serializer, new byte[10]);
    }

    [Test]
    public void TestFixedElementLengthArray()
    {
      Assert.True(typeof(int[]).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<int[]> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      TestArrayEqual(serializer, new[] { 1, 100, 1000, int.MaxValue, int.MinValue, -1 });
      TestArrayEqual(serializer, new int[0]);
      TestArrayEqual(serializer, new int[10]);
    }

    [Test]
    public void TestFixedElementLengthList()
    {
      Assert.True(typeof(List<int>).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<IList<int>> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      TestListEqual(serializer, new[] { 1, 100, 1000, int.MaxValue, int.MinValue, -1 });
      TestListEqual(serializer, new int[0]);
      TestListEqual(serializer, new int[10]);
    }

    [Test]
    public void TestNullableInt16()
    {
      Assert.True(typeof(short?).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<short?> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(null);
      serializer.TestEqual((short)0);
      serializer.TestEqual((short)1);
      serializer.TestEqual((short)-1);
      serializer.TestEqual(short.MaxValue);
      serializer.TestEqual(short.MinValue);
    }

    [Test]
    public void TestVariableByteArraySerializer()
    {
      Assert.True(typeof(byte[]).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<byte[]> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      TestArrayEqual(serializer, new byte[] { 1, 100, 128, byte.MaxValue, byte.MinValue });
      TestArrayEqual(serializer, new byte[0]);
      TestArrayEqual(serializer, new byte[10]);
    }

    [Test]
    public void TestVariableElementLengthArray()
    {
      Assert.True(typeof(string[]).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<string[]> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      TestArrayEqual(serializer, new[] { "abc", "äÖß☃", string.Empty });
      TestArrayEqual(serializer, new string[0]);
    }

    [Test]
    public void TestVariableElementLengthList()
    {
      Assert.True(typeof(List<string>).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<IList<string>> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      TestListEqual(serializer, new[] { "abc", "äÖß☃", string.Empty });
      TestListEqual(serializer, new string[0]);
    }
  }
}