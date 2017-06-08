using OculiService.Common.Tasks.Schedulers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public class SerializedDispatcher : ITaskDispatcher
  {
    public TaskScheduler TaskScheduler { get; private set; }

    public SerializedDispatcher()
    {
      this.TaskScheduler = (TaskScheduler) new OrderedTaskScheduler();
    }

    public Task QueueTask(Action action)
    {
      return Task.Factory.StartNew(action, new CancellationToken(), TaskCreationOptions.None, this.TaskScheduler);
    }

    public Task<T> QueueTask<T>(Func<T> action)
    {
      return Task.Factory.StartNew<T>(action, new CancellationToken(), TaskCreationOptions.None, this.TaskScheduler);
    }
  }
}
