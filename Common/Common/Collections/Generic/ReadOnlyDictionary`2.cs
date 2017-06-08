using System;using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OculiService.Common.Collections.Generic
{
  [ComVisible(false)]
  [DebuggerDisplay("Count = {Count}")]
  [DebuggerTypeProxy(typeof (ReadOnlyDictionaryDebugView<,>))]
  [Serializable]
  public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection
  {
    private readonly IDictionary<TKey, TValue> dictionary;
    private readonly bool isFixedSize;
    private readonly bool isSynchronized;
    private readonly object syncRoot;

    public ICollection<TKey> Keys
    {
      get
      {
        return this.dictionary.Keys;
      }
    }

    public ICollection<TValue> Values
    {
      get
      {
        return this.dictionary.Values;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        return this.dictionary[key];
      }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
      get
      {
        return this.dictionary[key];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public int Count
    {
      get
      {
        return this.dictionary.Count;
      }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get
      {
        return true;
      }
    }

    bool IDictionary.IsFixedSize
    {
      get
      {
        return this.isFixedSize;
      }
    }

    bool IDictionary.IsReadOnly
    {
      get
      {
        return true;
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        return (ICollection) this.dictionary.Keys;
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        return (ICollection) this.dictionary.Values;
      }
    }

    object IDictionary.this[object key]
    {
      get
      {
        return (object) this.dictionary[Util.Convert<TKey>(key, "key")];
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
        return this.isSynchronized;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return this.syncRoot;
      }
    }

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
      : this(dictionary, false)
    {
    }

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary, bool copy)
    {
      if (dictionary == null)
        throw new ArgumentNullException("dictionary");
      this.dictionary = copy ? (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>(dictionary) : dictionary;
      IDictionary dictionary1 = this.dictionary as IDictionary;
      this.isFixedSize = copy || dictionary1 != null && dictionary1.IsFixedSize;
      this.isSynchronized = dictionary1 != null && dictionary1.IsSynchronized;
      this.syncRoot = dictionary1 == null ? new object() : dictionary1.SyncRoot;
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      throw new NotSupportedException();
    }

    public bool ContainsKey(TKey key)
    {
      return this.dictionary.ContainsKey(key);
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      throw new NotSupportedException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return this.dictionary.TryGetValue(key, out value);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      throw new NotSupportedException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return this.dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      this.dictionary.CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return this.dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.dictionary.GetEnumerator();
    }

    void IDictionary.Add(object key, object value)
    {
      throw new NotSupportedException();
    }

    void IDictionary.Clear()
    {
      throw new NotSupportedException();
    }

    bool IDictionary.Contains(object key)
    {
      return this.dictionary.ContainsKey(Util.Convert<TKey>(key, "key"));
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new DictionaryEnumerator<TKey, TValue>(this.dictionary.GetEnumerator());
    }

    void IDictionary.Remove(object key)
    {
      throw new NotSupportedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      ICollection dictionary = this.dictionary as ICollection;
      if (dictionary != null)
        dictionary.CopyTo(array, index);
      else
        Util.CopyTo((ICollection) this, array, index);
    }
  }
}
