using System;
using System.IO;
using JetBrains.Annotations;

namespace NMaier.Serializers
{
  /// <summary>
  ///   Provides simple methods for serialization and deserialization of instances.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [PublicAPI]
  public interface ISerializer<T>
  {
    /// <summary>
    ///   Overhead in bytes. Should be 0 if unknown
    /// </summary>
    ushort Overhead { get; }

    /// <summary>
    ///   Size of a serialized item, including overhead. Should be 0 if unknown or variable
    /// </summary>
    ushort Size { get; }

    /// <summary>
    ///   Converts specified stream to the instance of T.
    /// </summary>
    /// <param name="stream">Bytes to convert</param>
    /// <returns>Resulting deserialized object</returns>
    [CanBeNull]
    T Deserialize([NotNull] Stream stream);

    /// <summary>
    ///   Converts specified bytes to the instance of T.
    /// </summary>
    /// <param name="bytes">Bytes to convert</param>
    /// <param name="offset">Offset into array</param>
    /// <param name="length">Length to take from array</param>
    /// <returns>Resulting deserialized object</returns>
    [CanBeNull]
    T Deserialize([NotNull] byte[] bytes, int offset, int length);

    /// <summary>
    ///   Converts specified bytes to the instance of T.
    /// </summary>
    /// <param name="bytes">Bytes to convert</param>
    /// <returns>Resulting deserialized object</returns>
    [CanBeNull]
    T Deserialize(ReadOnlySpan<byte> bytes);

    /// <summary>
    ///   Converts specified instance of T to byte array.
    /// </summary>
    /// <param name="obj">Instance to convert</param>
    /// <returns>Resulting serialized object</returns>
    [NotNull]
    byte[] Serialize([CanBeNull] T obj);
  }
}