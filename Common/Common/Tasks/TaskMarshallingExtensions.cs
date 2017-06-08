using System;using System.Threading.Tasks;

namespace OculiService.Common.Tasks
{
  public static class TaskMarshallingExtensions
  {
    public static bool UpdateTask(this TaskCompletionSource<string> source, ActivityStatusEntry status)
    {
      if (status.Status == ActivityCompletionStatus.Completed)
      {
        source.SetResult("Complete");
        return true;
      }
      if (status.Status == ActivityCompletionStatus.Faulted)
      {
        source.SetException((Exception) new ApplicationException(status.MessageId));
        return true;
      }
      if (status.Status != ActivityCompletionStatus.Canceled)
        return false;
      source.SetCanceled();
      return true;
    }
  }
}
