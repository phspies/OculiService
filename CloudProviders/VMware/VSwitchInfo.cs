using System;

namespace OculiService.CloudProviders.VMware
{
  public struct VSwitchInfo
  {
    public string Name;
    public string VirtualNetwork;
    public bool ConnectAtPowerOn;
  }
}
