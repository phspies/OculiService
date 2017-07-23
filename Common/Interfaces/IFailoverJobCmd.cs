using DoubleTake.Jobs.Contract;

namespace OculiService.Common.Interfaces
{
  public interface IFailoverJobCmd : ITaskCommandBase
  {
    void Invoke(FailoverOptions failoverOptions);
  }
}
