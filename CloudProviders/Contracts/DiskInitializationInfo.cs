using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class DiskInitializationInfo : IExtensibleDataObject
  {
    [DataMember]
    public string DiskIdentifier { get; set; }

    [DataMember]
    public bool ShouldPartitionDisk { get; set; }

    [DataMember]
    public string PartitioningScheme { get; set; }

    [DataMember]
    public PartitionInitializationInfo[] PartitionInitializationInfos { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
