using System;using System.Threading;
using System.Threading.Tasks;

namespace OculiService.Common.Threading
{
  public sealed class AsyncMutex
  {
    private readonly AsyncSemaphore semaphore;

    public AsyncMutex(bool initiallyOwned = false)
    {
      this.semaphore = new AsyncSemaphore(initiallyOwned ? 0 : 1, 1);
    }

    public AsyncMutex.Releaser Lock(CancellationToken cancellationToken)
    {
      this.semaphore.Wait(cancellationToken);
      return new AsyncMutex.Releaser(this.semaphore);
    }

    public AsyncMutex.Releaser Lock()
    {
      return this.Lock(CancellationToken.None);
    }

    public async Task<AsyncMutex.Releaser> LockAsync(CancellationToken cancellationToken)
    {
      await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
      return new AsyncMutex.Releaser(this.semaphore);
    }

    public async Task<AsyncMutex.Releaser> LockAsync()
    {
      await this.semaphore.WaitAsync().ConfigureAwait(false);
      return new AsyncMutex.Releaser(this.semaphore);
    }

    public bool TryLock(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.Wait(millisecondsTimeout, cancellationToken);
    }

    public bool TryLock(int millisecondsTimeout)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.Wait(millisecondsTimeout);
    }

    public bool TryLock(TimeSpan timeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.Wait(timeout, cancellationToken);
    }

    public bool TryLock(TimeSpan timeout)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.Wait(timeout);
    }

    public Task<bool> TryLockAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    public Task<bool> TryLockAsync(int millisecondsTimeout)
    {
      Invariant.ArgumentValidTimeout(millisecondsTimeout, "millisecondsTimeout");
      return this.semaphore.WaitAsync(millisecondsTimeout);
    }

    public Task<bool> TryLockAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.WaitAsync(timeout, cancellationToken);
    }

    public Task<bool> TryLockAsync(TimeSpan timeout)
    {
      Invariant.ArgumentValidTimeout(timeout, "timeout");
      return this.semaphore.WaitAsync(timeout);
    }

    public void Unlock()
    {
      this.semaphore.Release(1);
    }

    public struct Releaser : IDisposable
    {
      private AsyncSemaphore semaphore;

      internal Releaser(AsyncSemaphore semaphore)
      {
        this.semaphore = semaphore;
      }

      public void Dispose()
      {
        if (this.semaphore == null)
          return;
        this.semaphore.Release(1);
        this.semaphore = (AsyncSemaphore) null;
      }
    }
  }
}
