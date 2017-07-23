using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimDatacenter : IVimManagedItem
  {
    DatacenterProperties DatacenterProperties { get; set; }

    void GetCommonProperties(Dictionary<string, object> properties);

    void GetCommonProperties();

    ManagedObjectReference GetVmFolder();
  }
}
