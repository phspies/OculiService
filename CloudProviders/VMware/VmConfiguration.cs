﻿using System;

namespace OculiService.CloudProviders.VMware
{
  public struct VmConfiguration
  {
    public int NumCPU;
    public long MemoryMB;
    public VSwitchInfo[] NetworkAdapters;
    public int VmVersion;
  }
}
