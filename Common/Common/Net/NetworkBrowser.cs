using System;using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OculiService.Common.Net
{
  public static class NetworkBrowser
  {
    private const uint SV_TYPE_SERVER = 2;
    private const uint SV_TYPE_DOMAIN_ENUM = 2147483648;

    public static IEnumerable<string> FindDomains()
    {
      int dwResumeHandle = 0;
      IntPtr buffer = IntPtr.Zero;
      int entriesRead;
      int error;
      try
      {
        int dwTotalEntries;
        error = NativeMethods.NetServerEnum((string) null, 100, ref buffer, -1, out entriesRead, out dwTotalEntries, 2147483648U, (string) null, out dwResumeHandle);
      }
      catch (Exception ex)
      {
        throw;
      }
      try
      {
        if (error != 0)
          throw new Win32Exception(error);
        IntPtr zero = IntPtr.Zero;
        int sizeofINFO = Marshal.SizeOf(typeof (_SERVER_INFO_100));
        for (int i = 0; i < entriesRead; ++i)
          yield return ((_SERVER_INFO_100) Marshal.PtrToStructure(new IntPtr(buffer.ToInt64() + (long) (i * sizeofINFO)), typeof (_SERVER_INFO_100))).sv100_name;
      }
      finally
      {
        NativeMethods.NetApiBufferFree(buffer);
      }
    }

    public static IEnumerable<string> FindComputers(string domain)
    {
      int dwResumeHandle = 0;
      IntPtr buffer = IntPtr.Zero;
      int entriesRead;
      int num;
      try
      {
        int dwTotalEntries;
        num = NativeMethods.NetServerEnum((string) null, 100, ref buffer, -1, out entriesRead, out dwTotalEntries, 2U, domain, out dwResumeHandle);
      }
      catch (Exception ex)
      {
        throw;
      }
      try
      {
        if (num == 0)
        {
          IntPtr zero = IntPtr.Zero;
          int sizeofINFO = Marshal.SizeOf(typeof (_SERVER_INFO_100));
          for (int i = 0; i < entriesRead; ++i)
            yield return ((_SERVER_INFO_100) Marshal.PtrToStructure(new IntPtr(buffer.ToInt64() + (long) (i * sizeofINFO)), typeof (_SERVER_INFO_100))).sv100_name;
        }
      }
      finally
      {
        NativeMethods.NetApiBufferFree(buffer);
      }
    }
  }
}
