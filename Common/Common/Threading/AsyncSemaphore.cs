using System;using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Threading
{
  public sealed class AsyncSemaphore
  {
    private SemaphoreSlim semaphore;

    public int CurrentCount
    {
      get
      {
        return this.semaphore.CurrentCount;
      }
    }

    public AsyncSemaphore(int initialCount, int maxCount = 2147483647)
    {
      if (maxCount <= 0)
        throw new ArgumentOutOfRangeException("maxCount");
      if (initialCount < 0)
        throw new ArgumentOutOfRangeException("initialCount");
      if (initialCount > maxCount)
        throw new ArgumentOutOfRangeException("initialCount");
      this.semaphore = new SemaphoreSlim(initialCount, maxCount);
    }

    public int Release(int releaseCount = 1)
    {
      if (releaseCount < 1)
        throw new ArgumentOutOfRangeException("releaseCount");
      return this.semaphore.Release(releaseCount);
    }

    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.Wait(millisecondsTimeout, cancellationToken);
    }

    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.Wait(timeout, cancellationToken);
    }

    public bool Wait(int millisecondsTimeout)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.Wait(millisecondsTimeout);
    }

    public bool Wait(TimeSpan timeout)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.Wait(timeout);
    }

    public void Wait(CancellationToken cancellationToken)
    {
      this.semaphore.Wait(cancellationToken);
    }

    public void Wait()
    {
      this.semaphore.Wait();
    }

    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.WaitAsync(timeout, cancellationToken);
    }

    public Task<bool> WaitAsync(int millisecondsTimeout)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.WaitAsync(millisecondsTimeout, CancellationToken.None);
    }

    public Task<bool> WaitAsync(TimeSpan timeout)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.WaitAsync(timeout);
    }

    public Task WaitAsync(CancellationToken cancellationToken)
    {
      return this.semaphore.WaitAsync(cancellationToken);
    }

    public Task WaitAsync()
    {
      return this.semaphore.WaitAsync();
    }
  }
}
