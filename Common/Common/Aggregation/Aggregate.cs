using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OculiService.Common.Aggregation
{
  public sealed class Aggregate : IEnumerable<KeyValuePair<AggregateKey, object>>, IEnumerable
  {
    private ConcurrentDictionary<AggregateKey, Aggregate.Pair> aggregateData;

    public object this[Type type, string name]
    {
      get
      {
        try
        {
          return this.aggregateData[new AggregateKey(type, name)].Value0;
        }
        catch (KeyNotFoundException ex)
        {
          throw new KeyNotFoundException(string.Format("Aggregate key not found for data type {0} and name '{1}'", (object) type.FullName, (object) (name ?? string.Empty)), (Exception) ex);
        }
      }
      set
      {
        this.CheckType(type, "type");
        this.aggregateData[new AggregateKey(type, name)] = new Aggregate.Pair(value, true);
      }
    }

    public Aggregate()
    {
      this.aggregateData = new ConcurrentDictionary<AggregateKey, Aggregate.Pair>();
    }

    public void Clean()
    {
      foreach (Aggregate.Pair pair in (IEnumerable<Aggregate.Pair>) this.aggregateData.Values)
        pair.Value1 = false;
    }

    public object Get(Type type, string name)
    {
      return this[type, name];
    }

    public bool IsDirty(Type type, string name)
    {
      this.CheckType(type, "type");
      Aggregate.Pair pair;
      if (this.aggregateData.TryGetValue(new AggregateKey(type, name), out pair))
        return pair.Value1;
      return false;
    }

    public void Set(object data, string name)
    {
      this.CheckType(data.GetType(), "data");
      this[data.GetType(), name] = data;
    }

    public bool TryGet(Type type, string name, out object data)
    {
      this.CheckType(type, "type");
      Aggregate.Pair pair;
      if (this.aggregateData.TryGetValue(new AggregateKey(type, name), out pair))
      {
        data = pair.Value0;
        return true;
      }
      data = (object) null;
      return false;
    }

    public bool TryGetDirty(Type type, string name, out object data)
    {
      this.CheckType(type, "type");
      Aggregate.Pair pair;
      if (this.aggregateData.TryGetValue(new AggregateKey(type, name), out pair) && pair.Value1)
      {
        data = pair.Value0;
        return true;
      }
      data = (object) null;
      return false;
    }

    public IEnumerator<KeyValuePair<AggregateKey, object>> GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<AggregateKey, object>>) this.aggregateData.Select<KeyValuePair<AggregateKey, Aggregate.Pair>, KeyValuePair<AggregateKey, object>>((Func<KeyValuePair<AggregateKey, Aggregate.Pair>, KeyValuePair<AggregateKey, object>>) (pair => new KeyValuePair<AggregateKey, object>(pair.Key, pair.Value.Value0))).ToList<KeyValuePair<AggregateKey, object>>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.aggregateData.Select<KeyValuePair<AggregateKey, Aggregate.Pair>, KeyValuePair<AggregateKey, object>>((Func<KeyValuePair<AggregateKey, Aggregate.Pair>, KeyValuePair<AggregateKey, object>>) (pair => new KeyValuePair<AggregateKey, object>(pair.Key, pair.Value.Value0))).ToArray<KeyValuePair<AggregateKey, object>>().GetEnumerator();
    }

    private void CheckType(Type type, string paramName)
    {
      if (type.IsValueType)
        throw new ArgumentException("Data must not be a value type", paramName);
    }

    private class Pair
    {
      public object Value0 { get; set; }

      public bool Value1 { get; set; }

      public Pair(object value0, bool value1)
      {
        this.Value0 = value0;
        this.Value1 = value1;
      }
    }
  }
}
