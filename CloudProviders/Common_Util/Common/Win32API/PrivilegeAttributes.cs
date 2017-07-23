using System;

namespace Common_Util.Win32API
{
  [Flags]
  public enum PrivilegeAttributes : uint
  {
    EnabledByDefault = 1,
    Enabled = 2,
    Removed = 4,
    UsedForAccess = 2147483648,
  }
}
