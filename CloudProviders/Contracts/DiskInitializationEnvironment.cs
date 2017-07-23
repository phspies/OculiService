using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class DiskInitializationEnvironment : IExtensibleDataObject
  {
    public DiskInitializationInfo[] Disks;

    public int FirstDiskMaxSize { get; set; }

    public int OtherDisksMaxSize { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
