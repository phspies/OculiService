using System;using System.Collections.Generic;

namespace OculiService.Common
{
  public struct HashCode
  {
    private readonly int value;

    internal HashCode(int value)
    {
      this.value = value;
    }

    public static implicit operator int(HashCode hash)
    {
      return hash.GetHashCode();
    }

    public static bool operator ==(HashCode left, HashCode right)
    {
      return left.value == right.value;
    }

    public static bool operator !=(HashCode left, HashCode right)
    {
      return left.value != right.value;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is HashCode))
        return false;
      return this == (HashCode) obj;
    }

    public override int GetHashCode()
    {
      return this.value;
    }

    public static HashCode From<T>(T value)
    {
      return HashCode.From<T>(value, new Func<T, int>(HashCode.GetHashCode<T>));
    }

    public static HashCode From<T>(T value, IEqualityComparer<T> comparer)
    {
      return HashCode.From<T>(value, (Func<T, int>) (t => HashCode.GetHashCode<T>(t, comparer)));
    }

    public static HashCode From<T>(T value, Func<T, int> getHashCode)
    {
      return new HashCode(getHashCode(value));
    }

    public HashCode And<T>(T other)
    {
      return this.And<T>(other, new Func<T, int>(HashCode.GetHashCode<T>));
    }

    public HashCode And<T>(T other, IEqualityComparer<T> comparer)
    {
      return this.And<T>(other, (Func<T, int>) (t => HashCode.GetHashCode<T>(t, comparer)));
    }

    public HashCode And<T>(T other, Func<T, int> getHashCode)
    {
      int num1 = this.value;
      int num2 = 397;
      int num3 = num1 * num2 ^ getHashCode(other);
      return new HashCode(num1 * num3);
    }

    private static int GetHashCode<T>(T value)
    {
      if ((object) value == null)
        return 0;
      return value.GetHashCode();
    }

    private static int GetHashCode<T>(T value, IEqualityComparer<T> comparer)
    {
      if ((object) value == null)
        return 0;
      return comparer.GetHashCode(value);
    }
  }
}
