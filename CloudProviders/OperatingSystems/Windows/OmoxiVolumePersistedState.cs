using System.IO;

namespace OculiService.CloudProviders.OperatingSystems.Windows
{
    public class OculiServiceVolumePersistedState : OculiServiceVolumeOptions
    {
        public string OriginalPath { get; set; }
        public int VmSCSIUnitNumber { get; set; }
        public int VmSCSIBus { get; set; }
        public string VirtualDiskFilename { get; set; }
        public string DeviceID { get; set; }
        public int DriveIndex { get; set; }
        public string DriveName { get; set; }
        public string PNPDeviceID { get; set; }
        public string VolumeName { get; set; }
        public string MountPoint { get; set; }
        public long Size { get; set; }
        public byte[] ReverseVolumeSignature { get; set; }
        public bool IsGpt { get; set; }
        public static OculiServiceVolumePersistedState Create(OculiServiceVolumeOptions volume)
        {
            OculiServiceVolumePersistedState volumePersistedState = new OculiServiceVolumePersistedState();
            long availableFreeSpace = volume.AvailableFreeSpace;
            volumePersistedState.AvailableFreeSpace = availableFreeSpace;
            long desiredSize = volume.DesiredSize;
            volumePersistedState.DesiredSize = desiredSize;
            string diskControllerType = volume.DiskControllerType;
            volumePersistedState.DiskControllerType = diskControllerType;
            string provisioningType = volume.DiskProvisioningType;
            volumePersistedState.DiskProvisioningType = provisioningType;
            string driveFormat = volume.DriveFormat;
            volumePersistedState.DriveFormat = driveFormat;
            int driveType = (int)volume.DriveType;
            volumePersistedState.DriveType = (DriveType)driveType;
            int num3 = volume.IsSystemDrive ? 1 : 0;
            volumePersistedState.IsSystemDrive = num3 != 0;
            string label = volume.Label;
            volumePersistedState.Label = label;
            long totalSize = volume.TotalSize;
            volumePersistedState.TotalSize = totalSize;
            string virtualDiskPath = volume.VirtualDiskPath;
            volumePersistedState.VirtualDiskPath = virtualDiskPath;
            byte[] volumeSignature = volume.VolumeSignature;
            volumePersistedState.VolumeSignature = volumeSignature;
            string preexistingDiskPath = volume.PreexistingDiskPath;
            volumePersistedState.PreexistingDiskPath = preexistingDiskPath;
            return volumePersistedState;
        }
    }
}
