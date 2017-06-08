using OculiService.Common.Dispatchers;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace OculiService.Common.Concurrency
{
  public class TaskDispatcherScheduler : IScheduler
  {
    private readonly ITaskDispatcher dispatcher;

    public DateTimeOffset Now
    {
      get
      {
        return DateTimeOffset.Now;
      }
    }

    public TaskDispatcherScheduler(ITaskDispatcher dispatcher)
    {
      Invariant.ArgumentNotNull((object) dispatcher, "dispatcher");
      this.dispatcher = dispatcher;
    }

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      Invariant.ArgumentNotNull((object) action, "action");
      return this.Schedule<TState>(state, dueTime - DateTimeOffset.Now, action);
    }

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      Invariant.ArgumentNotNull((object) action, "action");
      return Scheduler.Default.Schedule<TState>(state, dueTime, (Func<IScheduler, TState, IDisposable>) ((scheduler, s) => this.Schedule<TState>(s, action)));
    }

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
      Invariant.ArgumentNotNull((object) action, "action");
      SingleAssignmentDisposable disposable = new SingleAssignmentDisposable();
      this.dispatcher.QueueTask((Action) (() =>
      {
        if (disposable.IsDisposed)
          return;
        disposable.Disposable = action((IScheduler) this, state);
      }));
      return (IDisposable) disposable;
    }
  }
}
