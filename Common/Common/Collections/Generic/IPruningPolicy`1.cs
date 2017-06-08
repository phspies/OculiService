namespace OculiService.Common.Collections.Generic
{
  public interface IPruningPolicy<T>
  {
    bool ShouldPrune(T item);

    bool ShouldPrune(int count, T item);
  }
}
