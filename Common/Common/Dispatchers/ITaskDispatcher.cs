using System;using System.Threading.Tasks;

namespace OculiService.Common.Dispatchers
{
  public interface ITaskDispatcher
  {
    Task QueueTask(Action action);

    Task<T> QueueTask<T>(Func<T> action);
  }
}
