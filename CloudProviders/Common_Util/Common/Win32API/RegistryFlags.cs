using System;

namespace Common_Util.Win32API
{
  [Flags]
  public enum RegistryFlags : uint
  {
    ForceRestore = 8,
    NoLazyFlush = 4,
    RefreshHive = 2,
    WholeHiveVolatile = 1,
  }
}
