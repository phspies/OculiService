using OculiService.Common;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace OculiService.Reactive.Linq
{
  public static class ObservableTimers
  {
    public static IObservable<long> PeriodicTimer(DateTimeOffset dueTime, TimeSpan period)
    {
      return ObservableTimers.PeriodicTimer(dueTime, period, (IScheduler) Scheduler.Default);
    }

    public static IObservable<long> PeriodicTimer(TimeSpan dueTime, TimeSpan period)
    {
      return ObservableTimers.PeriodicTimer(dueTime, period, (IScheduler) Scheduler.Default);
    }

    public static IObservable<long> PeriodicTimer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
    {
      Invariant.ArgumentNotNull((object) scheduler, "scheduler");
      TimeSpan p = ObservableTimers.Normalize(period);
      return Observable.Create<long>((Func<IObserver<long>, IDisposable>) (observer =>
      {
        long count = 0;
        return scheduler.Schedule(dueTime, (Action<Action<DateTimeOffset>>) (self =>
        {
          IObserver<long> observer1 = observer;
          long num1 = count;
          count = num1 + 1L;
          long num2 = num1;
          observer1.OnNext(num2);
          self(scheduler.Now + p);
        }));
      }));
    }

    public static IObservable<long> PeriodicTimer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
    {
      Invariant.ArgumentNotNull((object) scheduler, "scheduler");
      return Observable.Defer<long>((Func<IObservable<long>>) (() => ObservableTimers.PeriodicTimer(scheduler.Now + dueTime, period, scheduler)));
    }

    private static TimeSpan Normalize(TimeSpan timeSpan)
    {
      if (timeSpan.CompareTo(TimeSpan.Zero) < 0)
        return TimeSpan.Zero;
      return timeSpan;
    }
  }
}
