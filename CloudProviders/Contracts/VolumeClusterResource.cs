using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VolumeClusterResource : IExtensibleDataObject
  {
    public string Volume { get; set; }
    public ClusterResourceState ClusterResourceState { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
