using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public struct ManagedObjectAndProperties
  {
    public ManagedObjectReference ManagedObject;
    public Dictionary<string, object> Properties;
  }
}
