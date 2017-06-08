using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Logging
{
  [Serializable]
  internal sealed class ImmutableLogicalOperationStack<T> : IImmutableStack<T>, IEnumerable<T>, IEnumerable
  {
    public static readonly IImmutableStack<T> Empty = (IImmutableStack<T>) new ImmutableLogicalOperationStack<T>.EmptyStack();
    private IImmutableStack<T> _tail;
    private T _head;

    public bool IsEmpty
    {
      get
      {
        return false;
      }
    }

    private ImmutableLogicalOperationStack(T head, IImmutableStack<T> tail)
    {
      this._head = head;
      this._tail = tail;
    }

    public IImmutableStack<T> Push(T item)
    {
      return (IImmutableStack<T>) new ImmutableLogicalOperationStack<T>(item, (IImmutableStack<T>) this);
    }

    public IImmutableStack<T> Pop()
    {
      return this._tail;
    }

    public T Peek()
    {
      return this._head;
    }

    public IEnumerator<T> GetEnumerator()
    {
      IImmutableStack<T> stack;
      for (stack = (IImmutableStack<T>) this; !stack.IsEmpty; stack = stack.Pop())
        yield return stack.Peek();
      stack = (IImmutableStack<T>) null;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    [Serializable]
    private sealed class EmptyStack : IImmutableStack<T>, IEnumerable<T>, IEnumerable
    {
      public bool IsEmpty
      {
        get
        {
          return true;
        }
      }

      public IImmutableStack<T> Push(T value)
      {
        return (IImmutableStack<T>) new ImmutableLogicalOperationStack<T>(value, (IImmutableStack<T>) this);
      }

      public IImmutableStack<T> Pop()
      {
        throw new InvalidOperationException("Empty stack");
      }

      public T Peek()
      {
        throw new InvalidOperationException("Empty stack");
      }

      public IEnumerator<T> GetEnumerator()
      {
        yield break;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
