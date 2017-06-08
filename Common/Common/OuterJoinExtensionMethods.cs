using System;using System.Collections.Generic;

namespace OculiService.Common
{
  public static class OuterJoinExtensionMethods
  {
    public static OuterJoin<TLeft, TRight> OuterJoin<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, bool> selector) where TLeft : class where TRight : class
    {
      return new OuterJoin<TLeft, TRight>(left, right, selector);
    }
  }
}
