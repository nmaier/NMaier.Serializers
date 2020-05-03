using System;
using System.IO;
using NUnit.Framework;

namespace NMaier.Serializers.Tests
{
  internal static class Extensions
  {
    internal static void TestEqual<T>(this ISerializer<T> serializer, T val)
    {
      var s = serializer.Serialize(val);
      Assert.AreEqual(serializer.Deserialize(s.AsSpan()), val);
      Assert.AreEqual(serializer.Deserialize(s, 0, s.Length), val);
      using var ms = new MemoryStream(s, 0, s.Length, false);
      Assert.AreEqual(serializer.Deserialize(ms), val);
    }
  }
}