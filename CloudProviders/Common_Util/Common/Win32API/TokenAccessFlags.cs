using System;

namespace Common_Util.Win32API
{
  [Flags]
  public enum TokenAccessFlags : uint
  {
    StandardRightsRequired = 983040,
    StandardRightsRead = 131072,
    AssignPrimary = 1,
    Duplicate = 2,
    Impersonate = 4,
    Query = 8,
    QuerySource = 16,
    AdjustPrivileges = 32,
    AdjustGroups = 64,
    AdjustDefault = 128,
    AdjustSessionID = 256,
    Read = Query | StandardRightsRead,
    AllAccess = 983551,
  }
}
