using System;using System.Threading;

namespace OculiService.Common
{
  public static class ReaderWriterLockSlimExtensions
  {
    public static T ExecuteWithReadLock<T>(this ReaderWriterLockSlim mutex, Func<T> method)
    {
      mutex.EnterReadLock();
      try
      {
        return method();
      }
      finally
      {
        mutex.ExitReadLock();
      }
    }

    public static void ExecuteWithReadLock(this ReaderWriterLockSlim mutex, Action method)
    {
      mutex.EnterReadLock();
      try
      {
        method();
      }
      finally
      {
        mutex.ExitReadLock();
      }
    }

    public static void ExecuteWithWriteLock(this ReaderWriterLockSlim mutex, Action method)
    {
      mutex.EnterWriteLock();
      try
      {
        method();
      }
      finally
      {
        mutex.ExitWriteLock();
      }
    }

    public static T ExecuteWithWriteLock<T>(this ReaderWriterLockSlim mutex, Func<T> method)
    {
      mutex.EnterWriteLock();
      try
      {
        return method();
      }
      finally
      {
        mutex.ExitWriteLock();
      }
    }
  }
}
