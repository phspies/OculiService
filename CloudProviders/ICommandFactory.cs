namespace OculiService.Jobs.Commands
{
  public interface ICommandFactory
  {
    T Create<T>(string operation);

    object Create(string operation);
  }
}
