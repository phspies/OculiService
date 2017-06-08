using OculiService.Common.Tasks.Schedulers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public class SingleThreadedTaskDispatcher : ITaskDispatcher, IDisposable
  {
    private SingleThreadedTaskScheduler scheduler;

    public SingleThreadedTaskDispatcher(string threadName)
    {
      this.scheduler = new SingleThreadedTaskScheduler(threadName);
    }

    public Task QueueTask(Action action)
    {
      return Task.Factory.StartNew(action, new CancellationToken(), TaskCreationOptions.None, (TaskScheduler) this.scheduler);
    }

    public Task<T> QueueTask<T>(Func<T> action)
    {
      return Task.Factory.StartNew<T>(action, new CancellationToken(), TaskCreationOptions.None, (TaskScheduler) this.scheduler);
    }

    public void Dispose()
    {
      if (this.scheduler == null)
        return;
      this.scheduler.Dispose();
      this.scheduler = (SingleThreadedTaskScheduler) null;
    }
  }
}
