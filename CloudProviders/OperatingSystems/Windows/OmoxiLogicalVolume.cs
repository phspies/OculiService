using System.Runtime.Serialization;

namespace OculiService.CloudProviders.OperatingSystems.Windows
{
  public class OculiServiceLogicalVolume : OculiServiceVolumeOptions
  {
    public string LogicalVolumeName { get; set; }
  }
}
