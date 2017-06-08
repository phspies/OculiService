using System;using System.Runtime.InteropServices;
using System.Security;

namespace OculiService.Common.Net
{
  internal static class NativeMethods
  {
    [SuppressUnmanagedCodeSecurity]
    [DllImport("Netapi32", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int NetServerEnum(string ServerNane, int dwLevel, ref IntPtr pBuf, int dwPrefMaxLen, out int dwEntriesRead, out int dwTotalEntries, uint dwServerType, string domain, out int dwResumeHandle);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Netapi32", SetLastError = true)]
    internal static extern int NetApiBufferFree(IntPtr pBuf);
  }
}
