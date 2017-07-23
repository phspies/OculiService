using System;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.OperatingSystems.Windows
{
    public class OculiServiceVolumeOptions : OculiServiceVolume, ICloneable
    {
        public string VirtualDiskPath { get; set; }
        public long DesiredSize { get; set; }
        public string DiskControllerType { get; set; }
        public string DiskProvisioningType { get; set; }
        public byte[] VolumeSignature { get; set; }
        public string PreexistingDiskPath { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
