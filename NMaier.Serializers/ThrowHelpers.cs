using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NMaier.Serializers
{
  [DebuggerNonUserCode]
  internal static class ThrowHelpers
  {
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowArgumentException(string message, string name)
    {
      throw new ArgumentException(message, name);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowArgumentNullException(string name)
    {
      throw new ArgumentNullException(name);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowArgumentOutOfRangeException(string name)
    {
      throw new ArgumentOutOfRangeException(name);
    }
  }
}