using System;using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public class DefaultSchedulerTaskDispatcher : ITaskDispatcher
  {
    public Task QueueTask(Action action)
    {
      return Task.Factory.StartNew(action, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
    }

    public Task<T> QueueTask<T>(Func<T> action)
    {
      return Task.Factory.StartNew<T>(action, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
    }
  }
}
