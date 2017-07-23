using System;

namespace Oculi.Jobs.Context
{
  public class TaskInfoBatchManager : IDisposable
  {
    private ITaskInfoWrapper _JobInfoWrapper;

    public TaskInfoBatchManager(ITaskInfoWrapper jobInfoWrapper)
    {
      this._JobInfoWrapper = jobInfoWrapper;
      this._JobInfoWrapper.StartBatch();
    }

    public void Dispose()
    {
      this._JobInfoWrapper.EndBatch();
    }
  }
}
