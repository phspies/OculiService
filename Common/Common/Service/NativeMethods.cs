using System;using System.Runtime.InteropServices;

namespace OculiService.Common.Service
{
  internal static class NativeMethods
  {
    [DllImport("ADVAPI32.DLL")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetServiceStatus(IntPtr serviceHandle, ref Win32ServiceStatus serviceStatus);
  }
}
