using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VMQualificationResults : IExtensibleDataObject
  {
    public VHDInfo[] VHDInfo;
    public string DisplayName { get; set; }
    public string Address { get; set; }
    public bool IsHeartbeatInstalled { get; set; }
    public NetworkAdapterInfo[] NetworkAdapters { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
