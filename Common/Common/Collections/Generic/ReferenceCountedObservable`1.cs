using System;using System.Reactive.Disposables;

namespace OculiService.Common.Collections.Generic
{
  public class ReferenceCountedObservable<T> : IObservable<T>
  {
    private object _mutex;
    private IObservable<T> _observable;
    private int _subscriberCount;
    private Func<IObservable<T>> _onInitialSubscriber;
    private Action _onNoSubscribers;

    public int SubscriberCount
    {
      get
      {
        return this._subscriberCount;
      }
    }

    public ReferenceCountedObservable(Func<IObservable<T>> onInitialSubscriber, Action onNoSubscribers)
    {
      this._mutex = new object();
      this._subscriberCount = 0;
      this._onInitialSubscriber = onInitialSubscriber;
      this._onNoSubscribers = onNoSubscribers;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
      lock (this._mutex)
      {
        int local_3 = this._subscriberCount + 1;
        this._subscriberCount = local_3;
        if (local_3 == 1)
          this._observable = this._onInitialSubscriber();
      }
      IDisposable disposable;
      lock (this._mutex)
        disposable = this._observable.Subscribe(observer);
      return (IDisposable) new CompositeDisposable(new IDisposable[2]{ disposable, Disposable.Create((Action) (() =>
      {
        lock (this._mutex)
        {
          int local_2 = this._subscriberCount - 1;
          this._subscriberCount = local_2;
          if (local_2 != 0)
            return;
          this._observable = (IObservable<T>) null;
          this._onNoSubscribers();
        }
      })) });
    }
  }
}
