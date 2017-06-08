using OculiService.Win32.SafeHandles;
using System;

namespace OculiService.Win32
{
  public class RegistryKey
  {
    internal static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);
    internal static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
    internal static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
    internal static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);
    internal static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);
    internal static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);
    internal static readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);

    public static SafeRegistryHandle OpenBaseKey(RegistryHive hive)
    {
      return RegistryKey.GetBaseKey((IntPtr) ((int) hive));
    }

    internal static SafeRegistryHandle GetBaseKey(IntPtr hKey)
    {
      bool ownsHandle = hKey == RegistryKey.HKEY_PERFORMANCE_DATA;
      return new SafeRegistryHandle(hKey, ownsHandle);
    }
  }
}
