using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace OculiService.Common.Diagnostics
{
  internal static class NativeMethods
  {
    public const int LOGON32_LOGON_BATCH = 4;
    public const int LOGON32_LOGON_INTERACTIVE = 2;
    public const int LOGON32_PROVIDER_DEFAULT = 0;
    public const int CREATE_NO_WINDOW = 134217728;
    public const int CREATE_UNICODE_ENVIRONMENT = 1024;

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int LogonUser(string lpszUsername, string lpszDomain, IntPtr lpszPassword, int dwLogonType, int dwLogonProvider, out SafeFileHandle phToken);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int CreateProcessAsUser(SafeHandle hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, HandleRef lpEnvironment, string lpCurrentDirectory, NativeMethods.STARTUPINFO lpStartupInfo, NativeMethods.PROCESS_INFORMATION lpProcessInformation);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool CloseHandle(IntPtr handle);

    [StructLayout(LayoutKind.Sequential)]
    internal class STARTUPINFO
    {
      public int cb;
      public IntPtr lpReserved;
      public IntPtr lpDesktop;
      public IntPtr lpTitle;
      public int dwX;
      public int dwY;
      public int dwXSize;
      public int dwYSize;
      public int dwXCountChars;
      public int dwYCountChars;
      public int dwFillAttribute;
      public int dwFlags;
      public short wShowWindow;
      public short cbReserved2;
      public IntPtr lpReserved2;
      public SafeFileHandle hStdInput;
      public SafeFileHandle hStdOutput;
      public SafeFileHandle hStdError;

      public STARTUPINFO()
      {
        this.lpReserved = IntPtr.Zero;
        this.lpDesktop = IntPtr.Zero;
        this.lpTitle = IntPtr.Zero;
        this.lpReserved2 = IntPtr.Zero;
        this.hStdInput = new SafeFileHandle(IntPtr.Zero, false);
        this.hStdOutput = new SafeFileHandle(IntPtr.Zero, false);
        this.hStdError = new SafeFileHandle(IntPtr.Zero, false);
        this.cb = Marshal.SizeOf<NativeMethods.STARTUPINFO>(this);
      }

      public void Dispose()
      {
        if (this.hStdInput != null && !this.hStdInput.IsInvalid)
        {
          this.hStdInput.Close();
          this.hStdInput = (SafeFileHandle) null;
        }
        if (this.hStdOutput != null && !this.hStdOutput.IsInvalid)
        {
          this.hStdOutput.Close();
          this.hStdOutput = (SafeFileHandle) null;
        }
        if (this.hStdError == null || this.hStdError.IsInvalid)
          return;
        this.hStdError.Close();
        this.hStdError = (SafeFileHandle) null;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class PROCESS_INFORMATION
    {
      public IntPtr hProcess;
      public IntPtr hThread;
      public int dwProcessId;
      public int dwThreadId;

      public PROCESS_INFORMATION()
      {
        this.hProcess = IntPtr.Zero;
        this.hThread = IntPtr.Zero;
      }
    }
  }
}
