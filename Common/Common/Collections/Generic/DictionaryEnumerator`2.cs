using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  internal sealed class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IEnumerator
  {
    private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

    public DictionaryEntry Entry
    {
      get
      {
        return new DictionaryEntry(this.Key, this.Value);
      }
    }

    public object Key
    {
      get
      {
        return (object) this.enumerator.Current.Key;
      }
    }

    public object Value
    {
      get
      {
        return (object) this.enumerator.Current.Value;
      }
    }

    public object Current
    {
      get
      {
        return (object) this.Entry;
      }
    }

    public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
    {
      this.enumerator = enumerator;
    }

    public void Reset()
    {
      this.enumerator.Reset();
    }

    public bool MoveNext()
    {
      return this.enumerator.MoveNext();
    }
  }
}
