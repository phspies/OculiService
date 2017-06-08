using System;
namespace OculiService.Common.Diagnostics
{
  [Flags]
  public enum MiniDumpType
  {
    Normal = 0,
    WithDataSegs = 1,
    WithFullMemory = 2,
    WithHandleData = 4,
    FilterMemory = 8,
    ScanMemory = 16,
    WithUnloadedModules = 32,
    WithIndirectlyReferencedMemory = 64,
    FilterModulePaths = 128,
    WithProcessThreadData = 256,
    WithPrivateReadWriteMemory = 512,
    WithoutOptionalData = 1024,
    WithFullMemoryInfo = 2048,
    WithThreadInfo = 4096,
    WithCodeSegs = 8192,
    WithoutAuxiliaryState = 16384,
    WithFullAuxiliaryState = 32768,
    WithPrivateWriteCopyMemory = 65536,
    IgnoreInaccessibleMemory = 131072,
    WithTokenInformation = 262144,
  }
}
