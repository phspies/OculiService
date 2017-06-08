using System;
using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public class ImmediateDispatcher : ITaskDispatcher
  {
    public Task QueueTask(Action action)
    {
      TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
      action();
      
      object local = null;
      completionSource.SetResult((object) local);
      return (Task) completionSource.Task;
    }

    public Task<T> QueueTask<T>(Func<T> action)
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      T result = action();
      completionSource.SetResult(result);
      return completionSource.Task;
    }
  }
}
