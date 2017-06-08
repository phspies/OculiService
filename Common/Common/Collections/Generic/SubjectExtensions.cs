using System;using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OculiService.Common.Collections.Generic
{
  public static class SubjectExtensions
  {
    public static void Complete<T>(this ISubject<IObservable<T>> subject)
    {
      subject.OnNext(Observable.Empty<T>());
      subject.OnCompleted();
    }
  }
}
