using System;using System.Collections.Generic;
using System.Linq;

namespace OculiService.Common
{
  public class OuterJoin<TLeft, TRight> where TLeft : class where TRight : class
  {
    private readonly IEnumerable<TLeft> _left;
    private readonly IEnumerable<TRight> _right;
    private readonly Func<TLeft, TRight, bool> _selector;

    public OuterJoin(IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, bool> selector)
    {
      this._left = left;
      this._right = right;
      this._selector = selector;
    }

    public IEnumerable<TLeft> LeftOuter()
    {
      return this._left.SelectMany((Func<TLeft, IEnumerable<TRight>>) (l => this._right), (l, r) => new{ l = l, r = r }).Where(param0 => !this._selector(param0.l, param0.r)).Select(param0 => param0.l);
    }

    public IEnumerable<TRight> RightOuter()
    {
      return this._left.SelectMany((Func<TLeft, IEnumerable<TRight>>) (l => this._right), (l, r) => new{ l = l, r = r }).Where(param0 => !this._selector(param0.l, param0.r)).Select(param0 => param0.r);
    }

    public IEnumerable<Tuple<TLeft, TRight>> Inner()
    {
      return this._left.SelectMany((Func<TLeft, IEnumerable<TRight>>) (l => this._right), (l, r) => new{ l = l, r = r }).Where(param0 => this._selector(param0.l, param0.r)).Select(param0 => new Tuple<TLeft, TRight>(param0.l, param0.r));
    }

    public OuterJoinResult<TLeft, TRight> ToResult()
    {
      List<Tuple<TLeft, TRight>> source = new List<Tuple<TLeft, TRight>>();
      bool hasJoinMultiplicityGreaterThanOne = false;
      foreach (TLeft left1 in this._left)
      {
        TLeft left = left1;
        IEnumerable<Tuple<TLeft, TRight>> tuples = this._right.Where<TRight>((Func<TRight, bool>) (r => this._selector(left, r))).Select<TRight, Tuple<TLeft, TRight>>((Func<TRight, Tuple<TLeft, TRight>>) (r => new Tuple<TLeft, TRight>(left, r)));
        int num1 = tuples.Count<Tuple<TLeft, TRight>>();
        int num2 = 1;
        if (num1 < num2)
          source.Add(new Tuple<TLeft, TRight>(left, default (TRight)));
        else
          source.AddRange(tuples);
        int num3 = 1;
        if (num1 > num3)
          hasJoinMultiplicityGreaterThanOne = true;
      }
      foreach (TRight right1 in this._right)
      {
        TRight right = right1;
        int num = source.Where<Tuple<TLeft, TRight>>((Func<Tuple<TLeft, TRight>, bool>) (t => (object) right == (object) t.Item2)).Count<Tuple<TLeft, TRight>>();
        if (num < 1)
          source.Add(new Tuple<TLeft, TRight>(default (TLeft), right));
        else if (num > 1)
          hasJoinMultiplicityGreaterThanOne = true;
      }
      return new OuterJoinResult<TLeft, TRight>((IEnumerable<Tuple<TLeft, TRight>>) source, hasJoinMultiplicityGreaterThanOne);
    }
  }
}
