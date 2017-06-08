using System.Reactive;
using System.Threading.Tasks;

namespace OculiService.Common.Tasks
{
  public static class TaskConstants
  {
    private static readonly Task CompletedTask = (Task) Task.Factory.FromResult<Unit>(Unit.Default);
    private static readonly Task<bool> TrueTask = Task.Factory.FromResult<bool>(true);
    private static readonly Task<bool> FalseTask = Task.Factory.FromResult<bool>(false);

    public static Task Canceled
    {
      get
      {
        return (Task) TaskConstants<Unit>.Canceled;
      }
    }

    public static Task Completed
    {
      get
      {
        return TaskConstants.CompletedTask;
      }
    }

    public static Task<bool> True
    {
      get
      {
        return TaskConstants.TrueTask;
      }
    }

    public static Task<bool> False
    {
      get
      {
        return TaskConstants.FalseTask;
      }
    }
  }
}
