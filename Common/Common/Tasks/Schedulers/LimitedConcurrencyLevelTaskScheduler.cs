using System;using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Tasks.Schedulers
{
  public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
  {
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>();
    [ThreadStatic]
    private static HashSet<TaskScheduler> _processingSchedulers;
    private readonly int _maxDegreeOfParallelism;
    private int _delegatesQueuedOrRunning;

    private static HashSet<TaskScheduler> ProcessingSchedulers
    {
      get
      {
        if (LimitedConcurrencyLevelTaskScheduler._processingSchedulers == null)
          LimitedConcurrencyLevelTaskScheduler._processingSchedulers = new HashSet<TaskScheduler>(Enumerable.Empty<TaskScheduler>());
        return LimitedConcurrencyLevelTaskScheduler._processingSchedulers;
      }
    }

    private bool Processing
    {
      get
      {
        return LimitedConcurrencyLevelTaskScheduler.ProcessingSchedulers.Contains((TaskScheduler) this);
      }
      set
      {
        if (value)
        {
          if (LimitedConcurrencyLevelTaskScheduler.ProcessingSchedulers.Contains((TaskScheduler) this))
            return;
          LimitedConcurrencyLevelTaskScheduler.ProcessingSchedulers.Add((TaskScheduler) this);
        }
        else
          LimitedConcurrencyLevelTaskScheduler.ProcessingSchedulers.Remove((TaskScheduler) this);
      }
    }

    public override sealed int MaximumConcurrencyLevel
    {
      get
      {
        return this._maxDegreeOfParallelism;
      }
    }

    public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
    {
      if (maxDegreeOfParallelism < 1)
        throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
      this._maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    protected override sealed void QueueTask(Task task)
    {
      lock (this._tasks)
      {
        this._tasks.AddLast(task);
        if (this._delegatesQueuedOrRunning >= this._maxDegreeOfParallelism)
          return;
        this._delegatesQueuedOrRunning = this._delegatesQueuedOrRunning + 1;
        this.NotifyThreadPoolOfPendingWork();
      }
    }

    private void NotifyThreadPoolOfPendingWork()
    {
      ThreadPool.UnsafeQueueUserWorkItem((WaitCallback) (_ =>
      {
        this.Processing = true;
        try
        {
          while (true)
          {
            Task task;
            lock (this._tasks)
            {
              if (this._tasks.Count == 0)
              {
                this._delegatesQueuedOrRunning = this._delegatesQueuedOrRunning - 1;
                break;
              }
              task = this._tasks.First.Value;
              this._tasks.RemoveFirst();
            }
            this.TryExecuteTask(task);
          }
        }
        finally
        {
          this.Processing = false;
        }
      }), (object) null);
    }

    protected override sealed bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
      if (!this.Processing)
        return false;
      if (taskWasPreviouslyQueued)
        this.TryDequeue(task);
      return this.TryExecuteTask(task);
    }

    protected override sealed bool TryDequeue(Task task)
    {
      lock (this._tasks)
        return this._tasks.Remove(task);
    }

    protected override sealed IEnumerable<Task> GetScheduledTasks()
    {
      bool flag = false;
      try
      {
        flag = Monitor.TryEnter((object) this._tasks);
        if (flag)
          return (IEnumerable<Task>) this._tasks.ToArray<Task>();
        throw new NotSupportedException();
      }
      finally
      {
        if (flag)
          Monitor.Exit((object) this._tasks);
      }
    }
  }
}
