using System;using System.ComponentModel;

namespace OculiService.ComponentModel
{
  public interface IReactiveObject : INotifyPropertyChanged, IDisposable
  {
    IObservable<PropertyChangedEventInfo> PropertyChangedEvents { get; }
  }
}
