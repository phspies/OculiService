using System;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  [Serializable]
  public struct DatacenterProperties
  {
    public string Name;
    public int[] EffectiveRoles;
    public ManagedObjectReference VmFolder;
  }
}
