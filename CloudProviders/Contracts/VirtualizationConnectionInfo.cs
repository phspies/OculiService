using System;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VirtualizationConnectionInfo : IExtensibleDataObject
  {
    public bool Attach { get; set; }
    public string RepsetName { get; set; }
    public bool ProcessHardlinks { get; set; }
    public Guid MonitorId { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
