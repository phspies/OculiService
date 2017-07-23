using System.IO;

namespace OculiService.CloudProviders.OperatingSystems.Windows
{
    public class OculiServiceVolume
  {
    public string Label { get; set; }
    public string DriveFormat { get; set; }
    public DriveType DriveType { get; set; }
    public long AvailableFreeSpace { get; set; }
    public long TotalSize { get; set; }
    public bool IsSystemDrive { get; set; }
    public string VolumeType { get; set; }
    public bool IsSupported { get; set; }
    public bool ShortNameBehavior { get; set; }
  }
}
