using System;

namespace OculiService.CloudProviders.VMware
{
  [Serializable]
  public class FailoverConfigNic
  {
    public string Name;
    public string SourceNetwork;
    public string TargetNetwork;
    public bool ConnectedAtPowerOn;
  }
}
