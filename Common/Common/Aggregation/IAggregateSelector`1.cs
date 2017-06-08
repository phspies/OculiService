namespace OculiService.Common.Aggregation
{
  public interface IAggregateSelector<T>
  {
    T Select(Aggregate aggregate);
  }
}
