﻿using System;

namespace OculiService.CloudProviders.VMware
{
  [Serializable]
  public class FailoverConfig
  {
    public long SourceRamMB;
    public long TargetRamMB;
    public int SourceNumCPU;
    public int TargetNumCPU;
    public FailoverConfigNic[] NicMappings;
  }
}
