using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Common_Util.Win32API
{
  public class Win32Interface
  {
    private const int ANYSIZE_ARRAY = 1;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess([MarshalAs(UnmanagedType.U4)] ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentProcessId();

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool OpenProcessToken(IntPtr ProcessHandle, [MarshalAs(UnmanagedType.U4)] TokenAccessFlags dwDesiredAccess, out IntPtr TokenHandle);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref Win32Interface.TOKEN_PRIVILEGES NewState, int BufferLength, ref Win32Interface.TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES_SIMPLE NewState, int BufferLength, ref TOKEN_PRIVILEGES_SIMPLE PreviousState, ref int ReturnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("advapi32.dll", EntryPoint = "RegLoadKeyW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int RegLoadKey(IntPtr hKey, string subKey, string hive);

    [DllImport("advapi32.dll", EntryPoint = "RegUnLoadKeyW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int RegUnloadKey(IntPtr hKey, string subKey);

    [DllImport("advapi32.dll", EntryPoint = "RegCreateKeyW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int RegCreateKey(IntPtr hKey, string lpSubKey, out IntPtr phkResult);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern int RegCloseKey(IntPtr hKey);

    [DllImport("advapi32.dll", EntryPoint = "RegRestoreKeyW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int RegRestoreKey(IntPtr hKey, string lpFile, [MarshalAs(UnmanagedType.U4)] RegistryFlags dwFlags);

    public static void RestoreKey(string restoreKey, string savedKeyFile)
    {
      IntPtr phkResult;
      Win32Interface.TestRegistryResult(0, Win32Interface.RegCreateKey(HKey.LocalMachine, restoreKey, out phkResult), string.Format("Failed to create/open the key to restore (\"{0}\")", (object) restoreKey));
      int result = Win32Interface.RegRestoreKey(phkResult, savedKeyFile, RegistryFlags.ForceRestore);
      Win32Interface.RegCloseKey(phkResult);
      Win32Interface.TestRegistryResult(0, result, string.Format("Failed to restore the saved key in \"{0}\" to the key \"{1}\"", (object) savedKeyFile, (object) restoreKey));
    }

    public static void TestRegistryResult(int expected, int result, string msg)
    {
      if (result != expected)
      {
        Win32Exception win32Exception = new Win32Exception(result);
        StackTrace stackTrace = new StackTrace(1, true);
        int index = 0;
        MethodBase method;
        do
        {
          method = stackTrace.GetFrame(index).GetMethod();
          ++index;
        }
        while (method.Name == "TestRegistryResult");
        throw new Exception(string.Format("{0}: {1} Error Code = 0x{2:x} ({3})", (object) (method.ReflectedType.ToString() + "." + method.Name), (object) msg, (object) result, (object) win32Exception.Message));
      }
    }

    public struct TOKEN_PRIVILEGES
    {
      public uint PrivilegeCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
      public LUID_AND_ATTRIBUTES[] Privileges;
    }
  }
}
