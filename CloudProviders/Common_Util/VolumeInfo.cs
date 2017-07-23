
public class VolumeInfo
{

    public string DataStoreUrl;

    public string DriveLetter;

    public string Format;

    public string Label;

    public long DiskSizeMB;

    public long TargetDiskSizeMB;

    public long FreeSpaceMB;

    public long UsedSpaceMB;

    public bool IsSystemVolume;

    public int DriveType;

    public DiskController DiskCtrl;

    public int DiskType;

    public byte[] VolumeSignature;

    public string ReverseDataStoreUrl;

    public VolumeInfo()
    {
        this.DiskType = 0;
    }

    public VolumeInfo(VolumeInfo info)
    {
        this.DriveLetter = info.DriveLetter;
        this.Format = info.Format;
        this.Label = info.Label;
        this.DiskSizeMB = info.DiskSizeMB;
        this.IsSystemVolume = info.IsSystemVolume;
        this.DriveType = info.DriveType;
        this.DataStoreUrl = info.DataStoreUrl;
        this.TargetDiskSizeMB = info.TargetDiskSizeMB;
        this.FreeSpaceMB = info.FreeSpaceMB;
        this.UsedSpaceMB = info.UsedSpaceMB;
        this.DiskCtrl = info.DiskCtrl;
        this.DiskType = info.DiskType;
        this.VolumeSignature = info.VolumeSignature;
        this.ReverseDataStoreUrl = info.ReverseDataStoreUrl;
    }
}
