using System;using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public class CurrentOrDefaultSchedulerTaskDispatcher : ITaskDispatcher
  {
    public Task QueueTask(Action action)
    {
      return Task.Factory.StartNew(action);
    }

    public Task<T> QueueTask<T>(Func<T> action)
    {
      return Task.Factory.StartNew<T>(action);
    }
  }
}
