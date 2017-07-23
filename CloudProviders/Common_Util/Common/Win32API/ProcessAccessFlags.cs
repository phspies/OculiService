using System;

namespace Common_Util.Win32API
{
  [Flags]
  public enum ProcessAccessFlags : uint
  {
    All = 2035711,
    Terminate = 1,
    CreateThread = 2,
    VMOperation = 8,
    VMRead = 16,
    VMWrite = 32,
    DupHandle = 64,
    SetInformation = 512,
    QueryInformation = 1024,
    Synchronize = 1048576,
  }
}
