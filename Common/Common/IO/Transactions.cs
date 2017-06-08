using OculiService.Common.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace OculiService.Common.IO
{
  internal class Transactions : ITransactions
  {
    private static readonly Tracer _tracer = Tracer.GetTracer(typeof (Transactions));
    private static readonly Lazy<bool> _osSupportsTransactions = new Lazy<bool>(new Func<bool>(Transactions.OsSupportsTransactions));

    public bool TransactionsSupported
    {
      get
      {
        return false;
      }
    }

    private static bool OsSupportsTransactions()
    {
      try
      {
        using (new SafeFileHandle(Transactions.PlatformInvokes.CreateTransaction(IntPtr.Zero, IntPtr.Zero, 0U, 0U, 0U, 0U, "Testing KTM support"), true))
          ;
      }
      catch (Exception ex)
      {
        Transactions._tracer.TraceInformation(string.Format("KTM support check failed with error: {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public SafeFileHandle CreateTransaction()
    {
      IntPtr transaction = Transactions.PlatformInvokes.CreateTransaction(IntPtr.Zero, IntPtr.Zero, 0U, 0U, 0U, 0U, "Mgmt Svc Persistence");
      IntPtr invalidHandle = Transactions.PlatformInvokes.InvalidHandle;
      if (transaction == invalidHandle)
        throw new Win32Exception(Marshal.GetLastWin32Error());
      int num = 1;
      return new SafeFileHandle(transaction, num != 0);
    }

    public Stream Open(SafeFileHandle transaction, string path)
    {
      IntPtr fileTransacted = Transactions.PlatformInvokes.CreateFileTransacted(path, Transactions.PlatformInvokes.EFileAccess.GenericRead | Transactions.PlatformInvokes.EFileAccess.GenericWrite, Transactions.PlatformInvokes.EFileShare.Read, IntPtr.Zero, Transactions.PlatformInvokes.ECreationDisposition.CreateAlways, Transactions.PlatformInvokes.EFileAttributes.Normal, IntPtr.Zero, transaction.DangerousGetHandle(), IntPtr.Zero, IntPtr.Zero);
      IntPtr invalidHandle = Transactions.PlatformInvokes.InvalidHandle;
      if (fileTransacted == invalidHandle)
        throw new Win32Exception(Marshal.GetLastWin32Error());
      int num = 1;
      return (Stream) new FileStream(new SafeFileHandle(fileTransacted, num != 0), FileAccess.ReadWrite, 32768);
    }

    public void Commit(SafeFileHandle transaction)
    {
      if (!Transactions.PlatformInvokes.CommitTransaction(transaction.DangerousGetHandle()))
        throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public void Rollback(SafeFileHandle transaction)
    {
      if (!Transactions.PlatformInvokes.RollbackTransaction(transaction.DangerousGetHandle()))
        throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    private static class PlatformInvokes
    {
      public static IntPtr InvalidHandle = new IntPtr(-1);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr CreateFileTransacted(string name, Transactions.PlatformInvokes.EFileAccess desiredAccess, Transactions.PlatformInvokes.EFileShare sharedMode, IntPtr securityAttributes, Transactions.PlatformInvokes.ECreationDisposition creationDisposition, Transactions.PlatformInvokes.EFileAttributes flagsAndAttributes, IntPtr templateHandle, IntPtr transactionHandle, IntPtr miniVersion, IntPtr extendedParameter);

      [DllImport("KtmW32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr CreateTransaction(IntPtr securityAttributes, IntPtr uow, uint createOptions, uint isolationLevel, uint isolationFlags, uint timeout, string description);

      [DllImport("KtmW32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern bool CommitTransaction(IntPtr transactionHandle);

      [DllImport("KtmW32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern bool RollbackTransaction(IntPtr transactionHandle);

      public enum EMiniVersion : ushort
      {
        CommittedView = 0,
        DefaultView = 65534,
        DirtyView = 65535,
      }

      [Flags]
      public enum EFileAccess : uint
      {
        Delete = 65536,
        ReadControl = 131072,
        WriteDAC = 262144,
        WriteOwner = 524288,
        Synchronize = 1048576,
        StandardRightsRequired = WriteOwner | WriteDAC | ReadControl | Delete,
        StandardRightsRead = ReadControl,
        StandardRightsWrite = StandardRightsRead,
        StandardRightsExecute = StandardRightsWrite,
        StandardRightsAll = StandardRightsExecute | Synchronize | WriteOwner | WriteDAC | Delete,
        SpecificRightsAll = 65535,
        AccessSystemSecurity = 16777216,
        MaximumAllowed = 33554432,
        GenericRead = 2147483648,
        GenericWrite = 1073741824,
        GenericExecute = 536870912,
        GenericAll = 268435456,
      }

      [Flags]
      public enum EFileShare : uint
      {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
      }

      public enum ECreationDisposition : uint
      {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5,
      }

      [Flags]
      public enum EFileAttributes : uint
      {
        Readonly = 1,
        Hidden = 2,
        System = 4,
        Directory = 16,
        Archive = 32,
        Device = 64,
        Normal = 128,
        Temporary = 256,
        SparseFile = 512,
        ReparsePoint = 1024,
        Compressed = 2048,
        Offline = 4096,
        NotContentIndexed = 8192,
        Encrypted = 16384,
        Write_Through = 2147483648,
        Overlapped = 1073741824,
        NoBuffering = 536870912,
        RandomAccess = 268435456,
        SequentialScan = 134217728,
        DeleteOnClose = 67108864,
        BackupSemantics = 33554432,
        PosixSemantics = 16777216,
        OpenReparsePoint = 2097152,
        OpenNoRecall = 1048576,
        FirstPipeInstance = 524288,
      }
    }
  }
}
