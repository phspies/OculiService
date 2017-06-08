using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Threading
{
  public sealed class DeferrableOperation
  {
    private int count = 1;
    private TaskCompletionSource<Unit> taskCompletionSource;

    internal Task Task
    {
      get
      {
        if (this.taskCompletionSource != null)
          return (Task) this.taskCompletionSource.Task;
        return (Task) null;
      }
    }

    internal DeferrableOperation()
    {
    }

    public Deferral GetDeferral()
    {
      if (this.taskCompletionSource == null)
        this.taskCompletionSource = new TaskCompletionSource<Unit>();
      Interlocked.Increment(ref this.count);
      return new Deferral(this);
    }

    internal void Release()
    {
      if (Interlocked.Decrement(ref this.count) != 0 || this.taskCompletionSource == null)
        return;
      this.taskCompletionSource.TrySetResult(Unit.Default);
    }
  }
}
