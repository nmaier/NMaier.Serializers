using System;
using System.Text;
using NUnit.Framework;

namespace NMaier.Serializers.Tests
{
  public sealed class Plain
  {
    [Test]
    public void TestBool()
    {
      Assert.True(typeof(bool).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<bool> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(true);
      serializer.TestEqual(false);
    }

    [Test]
    public void TestByte()
    {
      Assert.True(typeof(byte).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<byte> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual((byte)0);
      serializer.TestEqual((byte)1);
      serializer.TestEqual(byte.MaxValue);
      serializer.TestEqual(byte.MinValue);
    }

    [Test]
    public void TestChar()
    {
      Assert.True(typeof(char).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<char> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual((char)0);
      serializer.TestEqual((char)1);
      serializer.TestEqual(char.MaxValue);
      serializer.TestEqual(char.MinValue);
    }

    [Test]
    public void TestDateTime()
    {
      Assert.True(typeof(DateTime).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<DateTime> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(DateTime.Now);
      serializer.TestEqual(DateTime.Today);
#if !NET48
      serializer.TestEqual(DateTime.UnixEpoch);
#endif
      serializer.TestEqual(DateTime.MinValue);
      serializer.TestEqual(DateTime.MaxValue);
    }

    [Test]
    public void TestDecimal()
    {
      Assert.True(typeof(decimal).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<decimal> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(0);
      serializer.TestEqual(new decimal(0.001));
      serializer.TestEqual(new decimal(2.2));
      serializer.TestEqual(decimal.MinValue);
      serializer.TestEqual(decimal.MaxValue);
    }

    [Test]
    public void TestGuid()
    {
      Assert.True(typeof(Guid).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<Guid> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(Guid.Empty);
      serializer.TestEqual(Guid.NewGuid());
      serializer.TestEqual(new Guid());
    }

    [Test]
    public void TestInt16()
    {
      Assert.True(typeof(short).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<short> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual((short)0);
      serializer.TestEqual((short)1);
      serializer.TestEqual((short)-1);
      serializer.TestEqual(short.MaxValue);
      serializer.TestEqual(short.MinValue);
    }

    [Test]
    public void TestInt32()
    {
      Assert.True(typeof(int).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<int> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(0);
      serializer.TestEqual(1);
      serializer.TestEqual(-1);
      serializer.TestEqual(int.MaxValue);
      serializer.TestEqual(int.MinValue);
    }

    [Test]
    public void TestInt64()
    {
      Assert.True(typeof(long).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<long> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(0);
      serializer.TestEqual(1);
      serializer.TestEqual(-1);
      serializer.TestEqual(long.MaxValue);
      serializer.TestEqual(long.MinValue);
    }

    [Test]
    public void TestSByte()
    {
      Assert.True(typeof(sbyte).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<sbyte> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual((sbyte)0);
      serializer.TestEqual((sbyte)1);
      serializer.TestEqual(sbyte.MaxValue);
      serializer.TestEqual(sbyte.MinValue);
    }

    [Test]
    public void TestString()
    {
      Assert.True(typeof(string).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<string> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null));
      serializer.TestEqual(string.Empty);
      serializer.TestEqual("abc");
      serializer.TestEqual("äÖß☃");
      var sb = new StringBuilder();
      for (var i = 0; i < 10; ++i) {
        sb.Append("äÖß☃");
      }

      serializer.TestEqual(sb.ToString());

      for (var i = 0; i < 10000; ++i) {
        sb.Append("äÖß☃");
      }

      serializer.TestEqual(sb.ToString());
      serializer.TestEqual(sb.ToString().Normalize(NormalizationForm.FormKD));
    }

    [Test]
    public void TestTimeSpan()
    {
      Assert.True(typeof(TimeSpan).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<TimeSpan> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(TimeSpan.Zero);
      serializer.TestEqual(TimeSpan.FromDays(2));
      serializer.TestEqual(TimeSpan.MinValue);
      serializer.TestEqual(TimeSpan.MaxValue);
    }

    [Test]
    public void TestUInt16()
    {
      Assert.True(typeof(ushort).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<ushort> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual((ushort)0);
      serializer.TestEqual((ushort)1);
      serializer.TestEqual(ushort.MaxValue);
      serializer.TestEqual(ushort.MinValue);
    }

    [Test]
    public void TestUInt32()
    {
      Assert.True(typeof(uint).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<uint> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(0U);
      serializer.TestEqual(1U);
      serializer.TestEqual(uint.MaxValue);
      serializer.TestEqual(uint.MinValue);
    }

    [Test]
    public void TestUInt64()
    {
      Assert.True(typeof(ulong).LookupSerializer(out var oserializer));
      if (!(oserializer is ISerializer<ulong> serializer)) {
        Assert.Fail("Invalid serializer returned");
        return;
      }

      serializer.TestEqual(0U);
      serializer.TestEqual(1U);
      serializer.TestEqual(ulong.MaxValue);
      serializer.TestEqual(ulong.MinValue);
    }
  }
}