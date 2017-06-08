using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public class SortedCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
  {
    private readonly object syncRoot = new object();
    private IComparer<T> comparer;
    private IList<T> items;

    protected IList<T> ItemsCollection
    {
      get
      {
        return this.items;
      }
    }

    public int Count
    {
      get
      {
        return this.items.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public T this[int index]
    {
      get
      {
        Invariant.ArgumentIsInRange<int>(index, "index", new Range<int>(0, this.Count - 1));
        return this.items[index];
      }
    }

    T IList<T>.this[int index]
    {
      get
      {
        Invariant.ArgumentIsInRange<int>(index, "index", new Range<int>(0, this.Count - 1));
        return this[index];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    bool IList.IsFixedSize
    {
      get
      {
        return false;
      }
    }

    object IList.this[int index]
    {
      get
      {
        return (object) this[index];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return this.syncRoot;
      }
    }

    protected SortedCollection(IList<T> list, IComparer<T> comparer)
    {
      Invariant.ArgumentNotNull((object) list, "list");
      this.comparer = comparer ?? (IComparer<T>) Comparer<T>.Default;
      this.items = list;
    }

    public SortedCollection()
      : this((IList<T>) new List<T>(), (IComparer<T>) null)
    {
    }

    public SortedCollection(IComparer<T> comparer)
      : this((IList<T>) new List<T>(), comparer)
    {
    }

    public SortedCollection(IEnumerable<T> collection)
      : this((IList<T>) new List<T>(collection), (IComparer<T>) null)
    {
    }

    public SortedCollection(IEnumerable<T> collection, IComparer<T> comparer)
      : this((IList<T>) new List<T>(collection), comparer)
    {
    }

    public void Add(T item)
    {
      Invariant.ArgumentNotNull((object) item, "item");
      this.InternalAdd(item);
    }

    private int InternalAdd(T item)
    {
      int index = this.items.BinarySearch<T>(item, this.comparer);
      if (index < 0)
        index = ~index;
      this.items.Insert(index, item);
      return index;
    }

    public void Clear()
    {
      this.items.Clear();
    }

    public bool Contains(T item)
    {
      Invariant.ArgumentNotNull((object) item, "item");
      return this.IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      Invariant.ArgumentNotNull((object) array, "array");
      this.items.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      Invariant.ArgumentNotNull((object) item, "item");
      int index = this.IndexOf(item);
      if (index < 0)
        return false;
      this.items.RemoveAt(index);
      return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public int IndexOf(T item)
    {
      Invariant.ArgumentNotNull((object) item, "item");
      int index = this.items.BinarySearch<T>(item, this.comparer);
      if (index >= 0)
      {
        while (!item.Equals((object) this.items[index]))
        {
          ++index;
          if (index >= this.items.Count || this.comparer.Compare(item, this.items[index]) != 0)
            goto label_4;
        }
        return index;
      }
label_4:
      return -1;
    }

    void IList<T>.Insert(int index, T item)
    {
      throw new NotSupportedException();
    }

    public void RemoveAt(int index)
    {
      Invariant.ArgumentIsInRange<int>(index, "index", new Range<int>(0, this.Count - 1));
      this.items.RemoveAt(index);
    }

    int IList.Add(object value)
    {
      Invariant.ArgumentNotNull(value, "item");
      if (SortedCollection<T>.IsCompatibleObject(value))
        return this.InternalAdd((T) value);
      return -1;
    }

    bool IList.Contains(object value)
    {
      if (SortedCollection<T>.IsCompatibleObject(value))
        return this.Contains((T) value);
      return false;
    }

    int IList.IndexOf(object value)
    {
      if (SortedCollection<T>.IsCompatibleObject(value))
        return this.IndexOf((T) value);
      return -1;
    }

    void IList.Insert(int index, object value)
    {
      throw new NotSupportedException();
    }

    void IList.Remove(object value)
    {
      if (!SortedCollection<T>.IsCompatibleObject(value))
        return;
      this.Remove((T) value);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      Invariant.ArgumentNotNull((object) array, "array");
      ((ICollection) this.items).CopyTo(array, index);
    }

    private static bool IsCompatibleObject(object value)
    {
      if (value is T)
        return true;
      if (value == null)
        return (object) default (T) == null;
      return false;
    }
  }
}
