using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VolumeQualificationResults : IExtensibleDataObject
  {
    public string DriveLetter { get; set; }
    public long DiskSize { get; set; }
    public long FreeSpace { get; set; }
    public long MaxFileSize { get; set; }
    public long ProvisionedSpace { get; set; }
    public bool IsSystemVolume { get; set; }
    public bool IsVolumeCSV { get; set; }
    public string Url { get; set; }
    public string CurrentOwnerNodeName { get; set; }
    public string ClusterResourceGroupName { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
