using System;
namespace OculiService.ComponentModel
{
  public interface IReactiveValue<T> : IObservable<T>, IDisposable
  {
    T Value { get; }
  }
}
