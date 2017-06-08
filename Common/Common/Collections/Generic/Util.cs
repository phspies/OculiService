using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public static class Util
  {
    public static void CopyTo(ICollection collection, Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index", "index must not be negative");
      if (index >= array.Length || collection.Count > array.Length - index)
        throw new ArgumentException("array is to small", "index");
      foreach (object obj in (IEnumerable) collection)
        array.SetValue(obj, index++);
    }

    public static bool IsCompatible<T>(object value)
    {
      return value is T || value == null && !typeof (T).IsValueType;
    }

    public static T Convert<T>(object value, string argumentName)
    {
      if (Util.IsCompatible<T>(value))
        return (T) value;
      throw new ArgumentException("WrongType", argumentName);
    }

    public static T[] CollectionToArray<T>(ICollection<T> c)
    {
      if (c == null || c.Count == 0)
        return new T[0];
      T[] array = new T[c.Count];
      c.CopyTo(array, 0);
      return array;
    }

    public static void AddOrReplace<K, V>(IDictionary<K, V> data, K key, V value)
    {
      if (data.ContainsKey(key))
        data[key] = value;
      else
        data.Add(key, value);
    }

    public static void AddOrReplace(IDictionary data, object key, object value)
    {
      if (data.Contains(key))
        data[key] = value;
      else
        data.Add(key, value);
    }

    public static void AddOrReplace<V>(ICollection<V> data, V value)
    {
      if (data.Contains(value))
        return;
      data.Add(value);
    }
  }
}
