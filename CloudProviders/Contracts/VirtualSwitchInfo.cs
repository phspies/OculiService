using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VirtualSwitchInfo : IExtensibleDataObject
  {
    public string SwitchUuid { get; set; }
    public string Label { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
