using OculiService.Jobs.Context;

namespace OculiService.Jobs.Commands
{
  public interface ITaskCommandBase
  {
    string Name { get; }

    TaskContext Context { get; }
  }
}
