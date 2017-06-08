using System.Threading.Tasks;

namespace OculiService.Common.Tasks
{
  public static class TaskConstants<T>
  {
    private static readonly Task<T> CanceledTask;

    public static Task<T> Canceled
    {
      get
      {
        return TaskConstants<T>.CanceledTask;
      }
    }

    static TaskConstants()
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetCanceled();
      TaskConstants<T>.CanceledTask = completionSource.Task;
    }
  }
}
