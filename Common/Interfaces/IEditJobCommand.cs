using DoubleTake.Jobs.Contract;
using OculiService.Common.Contract;

namespace OculiService.Common.Interfaces
{
  public interface IEditJobCommand : ITaskCommandBase
  {
    void Invoke(TaskOptions jobOptions);
  }
}
