using System;using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OculiService.Common.Extensibility
{
  public class ExtensionCollection<T> : IExtensionCollection<T>, ICollection<IExtension<T>>, IEnumerable<IExtension<T>>, IEnumerable where T : class, IExtensibleObject<T>
  {
    private readonly List<IExtension<T>> items = new List<IExtension<T>>();
    private readonly T owner;
    private readonly object syncRoot;

    public T this[string name]
    {
      get
      {
        lock (this.syncRoot)
        {
          foreach (T item_0 in this)
          {
            Type local_4 = item_0.GetType();
            ExtensionNameAttribute local_5 = local_4.GetCustomAttributes(typeof (ExtensionNameAttribute), false).Cast<ExtensionNameAttribute>().FirstOrDefault<ExtensionNameAttribute>();
            if (local_5 != null)
            {
              if (local_5.Name == name)
                return item_0;
            }
            else if (local_4.Name == name || local_4.Name == name + "Extension")
              return item_0;
          }
        }
        return default (T);
      }
    }

    public int Count
    {
      get
      {
        lock (this.syncRoot)
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

    public ExtensionCollection(T owner)
      : this(owner, new object())
    {
    }

    public ExtensionCollection(T owner, object syncRoot)
    {
      if ((object) owner == null)
        throw new ArgumentNullException("owner");
      if (syncRoot == null)
        throw new ArgumentNullException("syncRoot");
      this.owner = owner;
      this.syncRoot = syncRoot;
    }

    public TExtension Find<TExtension>() where TExtension : T
    {
      lock (this.syncRoot)
      {
        foreach (T item_0 in this)
        {
          if ((object) item_0 is TExtension)
            return (TExtension) (object) item_0;
        }
      }
      return default (TExtension);
    }

    public Collection<TExtension> FindAll<TExtension>() where TExtension : T
    {
      Collection<TExtension> collection = new Collection<TExtension>();
      lock (this.syncRoot)
      {
        foreach (T item_0 in this)
        {
          if ((object) item_0 is TExtension)
            collection.Add((TExtension) (object) item_0);
        }
      }
      return collection;
    }

    public void Add(IExtension<T> item)
    {
      if (item == null)
        throw new ArgumentNullException("item");
      lock (this.syncRoot)
      {
        item.Attach(this.owner);
        this.items.Add(item);
      }
    }

    public void Clear()
    {
      lock (this.syncRoot)
      {
        IExtension<T>[] local_2 = new IExtension<T>[this.items.Count];
        this.items.CopyTo(local_2, 0);
        this.items.Clear();
        foreach (IExtension<T> item_0 in local_2)
          item_0.Detach(this.owner);
      }
    }

    public bool Contains(IExtension<T> item)
    {
      lock (this.syncRoot)
        return this.items.Contains(item);
    }

    public void CopyTo(IExtension<T>[] array, int arrayIndex)
    {
      lock (this.syncRoot)
        this.items.CopyTo(array, arrayIndex);
    }

    public bool Remove(IExtension<T> item)
    {
      lock (this.syncRoot)
      {
        int local_2 = this.items.IndexOf(item);
        if (local_2 >= 0)
        {
          item.Detach(this.owner);
          this.items.RemoveAt(local_2);
        }
        return false;
      }
    }

    public IEnumerator<IExtension<T>> GetEnumerator()
    {
      lock (this.syncRoot)
        return (IEnumerator<IExtension<T>>) this.items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
