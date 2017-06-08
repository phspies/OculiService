using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OculiService.Common.Alternate
{
  [DebuggerDisplay("Tuple({Item1}, {Item2}, {Item3})")]
  [Serializable]
  public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
  {
    public T1 Item1 { get; private set; }

    public T2 Item2 { get; private set; }

    public T3 Item3 { get; private set; }

    public Tuple(T1 item1, T2 item2, T3 item3)
    {
      this.Item1 = item1;
      this.Item2 = item2;
      this.Item3 = item3;
    }

    public bool Equals(Tuple<T1, T2, T3> other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      if (EqualityComparer<T1>.Default.Equals(other.Item1, this.Item1) && EqualityComparer<T2>.Default.Equals(other.Item2, this.Item2))
        return EqualityComparer<T3>.Default.Equals(other.Item3, this.Item3);
      return false;
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as Tuple<T1, T2, T3>);
    }

    public override int GetHashCode()
    {
      HashCode hashCode = HashCode.From<T1>(this.Item1);
      hashCode = hashCode.And<T2>(this.Item2);
      return (int) hashCode.And<T3>(this.Item3);
    }
  }
}
