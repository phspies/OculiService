using OculiService.Common.ExceptionHandling;
using System;
using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public static class TaskDispatcherExtensions
  {
    public static void ExecuteTask(this ITaskDispatcher self, Action action)
    {
      self.ExecuteTask(action, -1);
    }

    public static void ExecuteTask(this ITaskDispatcher self, Action action, TimeSpan timeout)
    {
      try
      {
        self.QueueTask(action).Wait(timeout);
      }
      catch (AggregateException ex)
      {
        throw ex.Flatten().InnerException.PrepareForRethrow();
      }
    }

    public static void ExecuteTask(this ITaskDispatcher self, Action action, int timeoutMs)
    {
      try
      {
        self.QueueTask(action).Wait(timeoutMs);
      }
      catch (AggregateException ex)
      {
        throw ex.Flatten().InnerException.PrepareForRethrow();
      }
    }

    public static T ExecuteTask<T>(this ITaskDispatcher self, Func<T> action)
    {
      return self.ExecuteTask<T>(action, -1);
    }

    public static T ExecuteTask<T>(this ITaskDispatcher self, Func<T> action, TimeSpan timeout)
    {
      try
      {
        using (Task<T> task = self.QueueTask<T>(action))
        {
          if (!task.Wait(timeout))
            throw new TimeoutException("Timed out waiting on task");
          return task.Result;
        }
      }
      catch (AggregateException ex)
      {
        throw ex.Flatten().InnerException.PrepareForRethrow();
      }
    }

    public static T ExecuteTask<T>(this ITaskDispatcher self, Func<T> action, int timeoutMs)
    {
      try
      {
        using (Task<T> task = self.QueueTask<T>(action))
        {
          if (!task.Wait(timeoutMs))
            throw new TimeoutException("Timed out waiting on task");
          return task.Result;
        }
      }
      catch (AggregateException ex)
      {
        throw ex.Flatten().InnerException.PrepareForRethrow();
      }
    }
  }
}
