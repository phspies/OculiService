using System;using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public class TaskDispatcher<TScheduler> : ITaskDispatcher, IDisposable where TScheduler : TaskScheduler, new()
  {
    private TScheduler scheduler;

    public TaskScheduler TaskScheduler
    {
      get
      {
        return (TaskScheduler) this.scheduler;
      }
    }

    public TaskDispatcher()
    {
      this.scheduler = Activator.CreateInstance<TScheduler>();
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
      if ((object) this.scheduler == null)
        return;
      IDisposable scheduler = (object) this.scheduler as IDisposable;
      if (scheduler != null)
        scheduler.Dispose();
      this.scheduler = default (TScheduler);
    }
  }
}
