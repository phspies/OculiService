using System;using System.Collections.Generic;
using System.Linq;

namespace OculiService.Common
{
  public class OuterJoinResult<TLeft, TRight> where TLeft : class where TRight : class
  {
    public bool HasJoinMultiplicityGreaterThanOne { get; private set; }

    public IEnumerable<Tuple<TLeft, TRight>> Joins { get; private set; }

    public OuterJoinResult(IEnumerable<Tuple<TLeft, TRight>> joins, bool hasJoinMultiplicityGreaterThanOne)
    {
      this.HasJoinMultiplicityGreaterThanOne = hasJoinMultiplicityGreaterThanOne;
      this.Joins = joins;
    }

    public IEnumerable<TLeft> LeftOuter()
    {
      return this.Joins.Where<Tuple<TLeft, TRight>>((Func<Tuple<TLeft, TRight>, bool>) (j => (object) j.Item2 == null)).Select<Tuple<TLeft, TRight>, TLeft>((Func<Tuple<TLeft, TRight>, TLeft>) (j => j.Item1));
    }

    public IEnumerable<TRight> RightOuter()
    {
      return this.Joins.Where<Tuple<TLeft, TRight>>((Func<Tuple<TLeft, TRight>, bool>) (j => (object) j.Item1 == null)).Select<Tuple<TLeft, TRight>, TRight>((Func<Tuple<TLeft, TRight>, TRight>) (j => j.Item2));
    }

    public IEnumerable<Tuple<TLeft, TRight>> Inner()
    {
      return this.Joins.Where<Tuple<TLeft, TRight>>((Func<Tuple<TLeft, TRight>, bool>) (j =>
      {
        if ((object) j.Item1 != null)
          return (object) j.Item2 != null;
        return false;
      }));
    }
  }
}
