namespace OculiService.Jobs.Commands
{
  public interface ResultsAvailable<T>
  {
    T Results { get; }
  }
}
