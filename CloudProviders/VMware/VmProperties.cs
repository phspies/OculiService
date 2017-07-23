using System;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public struct VmProperties
  {
    public string Uuid;
    public string Name;
    public string GuestId;
    public string GuestFullName;
    public string HostName;
    public VirtualMachinePowerState PowerState;
    public bool IsTemplate;
    public int NumCPU;
    public int MemoryMB;
    public string Version;
  }
}
