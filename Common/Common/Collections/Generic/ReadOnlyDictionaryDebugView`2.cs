using System;using System.Collections.Generic;
using System.Diagnostics;

namespace OculiService.Common.Collections.Generic
{
  internal sealed class ReadOnlyDictionaryDebugView<TKey, TValue>
  {
    private readonly IDictionary<TKey, TValue> dictionary;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<TKey, TValue>[] Items
    {
      get
      {
        KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[this.dictionary.Count];
        this.dictionary.CopyTo(array, 0);
        return array;
      }
    }

    public ReadOnlyDictionaryDebugView(ReadOnlyDictionary<TKey, TValue> dictionary)
    {
      if (dictionary == null)
        throw new ArgumentNullException("dictionary");
      this.dictionary = (IDictionary<TKey, TValue>) dictionary;
    }
  }
}
