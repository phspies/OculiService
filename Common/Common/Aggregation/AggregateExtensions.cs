namespace OculiService.Common.Aggregation
{
  public static class AggregateExtensions
  {
    public static Aggregate Accumulate<TData>(this Aggregate aggregate, TData data, string name) where TData : class
    {
      aggregate[typeof (TData), name] = (object) data;
      return aggregate;
    }

    public static TData Get<TData>(this Aggregate aggregate, string name) where TData : class
    {
      return (TData) aggregate.Get(typeof (TData), name);
    }

    public static bool IsDirty<TData>(this Aggregate aggregate, string name) where TData : class
    {
      return aggregate.IsDirty(typeof (TData), name);
    }

    public static bool TryGet<TData>(this Aggregate aggregate, string name, out TData data) where TData : class
    {
      object data1;
      if (aggregate.TryGet(typeof (TData), name, out data1))
      {
        data = (TData) data1;
        return true;
      }
      data = default (TData);
      return false;
    }

    public static bool TryGetDirty<TData>(this Aggregate aggregate, string name, out TData data) where TData : class
    {
      object data1;
      if (aggregate.TryGetDirty(typeof (TData), name, out data1))
      {
        data = (TData) data1;
        return true;
      }
      data = default (TData);
      return false;
    }
  }
}
