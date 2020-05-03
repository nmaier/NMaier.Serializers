using System;
using System.Collections.Generic;

namespace NMaier.Serializers
{
  internal static class Builtins
  {
    internal static readonly IReadOnlyDictionary<Type, object> Serializers = new Dictionary<Type, object> {
      { typeof(bool), new BoolSerializer() },
      { typeof(byte), new ByteSerializer() },
      { typeof(byte[]), new VariableByteArraySerializer() },
      { typeof(sbyte), new SByteSerializer() },
      { typeof(char), new CharSerializer() },
      { typeof(short), new Int16Serialzer() },
      { typeof(ushort), new UInt16Serialzer() },
      { typeof(int), new Int32Serialzer() },
      { typeof(uint), new UInt32Serialzer() },
      { typeof(long), new Int64Serialzer() },
      { typeof(ulong), new UInt64Serialzer() },
      { typeof(float), new FloatSerialzer() },
      { typeof(double), new DoubleSerialzer() },
      { typeof(decimal), new DecimalSerializer() },
      { typeof(DateTime), new DateTimeSerialzer() },
      { typeof(TimeSpan), new TimeSpanSerialzer() },
      { typeof(Guid), new GuidSerializer() },
      { typeof(string), new StringSerializer() }
    };
  }
}