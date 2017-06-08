using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OculiService.Common.Alternate
{
  [DebuggerDisplay("Tuple({Item1}, {Item2})")]
  [Serializable]
  public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
  {
    private T1 Value0 { get; set; }

    private T2 Value1 { get; set; }

    public T1 Item1
    {
      get
      {
        return this.Value0;
      }
      private set
      {
        this.Value0 = value;
      }
    }

    public T2 Item2
    {
      get
      {
        return this.Value1;
      }
      private set
      {
        this.Value1 = value;
      }
    }

    public Tuple(T1 item1, T2 item2)
    {
      this.Item1 = item1;
      this.Item2 = item2;
    }

    public bool Equals(Tuple<T1, T2> other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      if (EqualityComparer<T1>.Default.Equals(other.Item1, this.Item1))
        return EqualityComparer<T2>.Default.Equals(other.Item2, this.Item2);
      return false;
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as Tuple<T1, T2>);
    }

    public override int GetHashCode()
    {
      return (int) HashCode.From<T1>(this.Item1).And<T2>(this.Item2);
    }
  }
}
