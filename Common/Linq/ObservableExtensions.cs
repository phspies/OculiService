using OculiService.Common.Aggregation;
using System;
using System.Reactive.Linq;

namespace OculiService.Linq
{
  public static class ObservableExtensions
  {
    public static IObservable<Aggregate> Scan<TData>(this IObservable<TData> source, Aggregate seed, string name) where TData : class
    {
      return source.Scan<TData, Aggregate>(seed, (Func<Aggregate, TData, Aggregate>) ((aggregate, data) => aggregate.Accumulate<TData>(data, name)));
    }

    public static IObservable<Aggregate> CombineLatest<TData>(this IObservable<Aggregate> leftSource, IObservable<TData> rightSource, string name) where TData : class
    {
      return leftSource.CombineLatest<Aggregate, TData, Aggregate>(rightSource, (Func<Aggregate, TData, Aggregate>) ((aggregate, data) => aggregate.Accumulate<TData>(data, name)));
    }
  }
}
