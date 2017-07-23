using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VirtualSwitchMapping : IExtensibleDataObject
  {
    public VirtualSwitchInfo VirtualSwitch { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
