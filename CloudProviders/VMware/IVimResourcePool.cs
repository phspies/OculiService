using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimResourcePool : IVimManagedItem
  {
    ManagedObjectReference Parent { get; set; }

    void GetCommonProperties(Dictionary<string, object> properties);
  }
}
