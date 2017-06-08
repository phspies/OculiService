using System;using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Tasks.Schedulers
{
  public sealed class SingleThreadedTaskScheduler : TaskScheduler, IDisposable
  {
    private readonly Thread thread;
    private BlockingCollection<Task> tasks;

    public override sealed int MaximumConcurrencyLevel
    {
      get
      {
        return 1;
      }
    }

    public SingleThreadedTaskScheduler() : this("Single-threaded task scheduler")
    {
    }

    public SingleThreadedTaskScheduler(string threadName)
    {
      this.tasks = new BlockingCollection<Task>();
      this.thread = new Thread((ThreadStart) (() =>
      {
        foreach (Task consuming in this.tasks.GetConsumingEnumerable())
          this.TryExecuteTask(consuming);
      }));
      this.thread.IsBackground = true;
      this.thread.Name = threadName;
      this.thread.Start();
    }

    public void Dispose()
    {
      if (this.tasks == null)
        return;
      this.tasks.CompleteAdding();
      this.thread.Join();
      this.tasks.Dispose();
      this.tasks = (BlockingCollection<Task>) null;
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
      return (IEnumerable<Task>) this.tasks.ToArray();
    }

    protected override void QueueTask(Task task)
    {
      this.tasks.Add(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
      if (Thread.CurrentThread == this.thread)
        return this.TryExecuteTask(task);
      return false;
    }
  }
}
