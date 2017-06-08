using OculiService.Win32.SafeHandles;

namespace OculiService.Win32
{
  public static class Registry
  {
    public static readonly SafeRegistryHandle CurrentUser = RegistryKey.GetBaseKey(RegistryKey.HKEY_CURRENT_USER);
    public static readonly SafeRegistryHandle LocalMachine = RegistryKey.GetBaseKey(RegistryKey.HKEY_LOCAL_MACHINE);
    public static readonly SafeRegistryHandle ClassesRoot = RegistryKey.GetBaseKey(RegistryKey.HKEY_CLASSES_ROOT);
    public static readonly SafeRegistryHandle Users = RegistryKey.GetBaseKey(RegistryKey.HKEY_USERS);
    public static readonly SafeRegistryHandle PerformanceData = RegistryKey.GetBaseKey(RegistryKey.HKEY_PERFORMANCE_DATA);
    public static readonly SafeRegistryHandle CurrentConfig = RegistryKey.GetBaseKey(RegistryKey.HKEY_CURRENT_CONFIG);
  }
}
