using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static class TaskCompletionSourceExtensions
    {
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> self, Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    Task<TResult> task1 = task as Task<TResult>;
                    self.SetResult(task1 != null ? task1.Result : default(TResult));
                    break;
                case TaskStatus.Canceled:
                    self.SetCanceled();
                    break;
                case TaskStatus.Faulted:
                    self.SetException((IEnumerable<Exception>)task.Exception.InnerExceptions);
                    break;
                default:
                    throw new InvalidOperationException("Task is not completed.");
            }
        }

        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> self, Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    Task<TResult> task1 = task as Task<TResult>;
                    return self.TrySetResult(task1 != null ? task1.Result : default(TResult));
                case TaskStatus.Canceled:
                    return self.TrySetCanceled();
                case TaskStatus.Faulted:
                    return self.TrySetException((IEnumerable<Exception>)task.Exception.InnerExceptions);
                default:
                    throw new InvalidOperationException("Task is not completed.");
            }
        }
    }
}
