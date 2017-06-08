using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Logging
{
  internal interface IImmutableStack<T> : IEnumerable<T>, IEnumerable
  {
    bool IsEmpty { get; }

    IImmutableStack<T> Push(T value);

    IImmutableStack<T> Pop();

    T Peek();
  }
}
