namespace OculiService.Common.Interfaces
{
    public interface ITaskCommandBase
  {
    string Name { get; }

    JobContext Context { get; }
  }
}
