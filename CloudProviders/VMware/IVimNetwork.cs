using System.Collections.Generic;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimNetwork : IVimManagedItem
  {
    bool IsDistributed { get; }

    string PortgroupKey { get; }

    NetworkProperties Properties { get; set; }

    void GetCommonProperties(Dictionary<string, object> properties);

    NetworkProperties GetCommonProperties();
  }
}
