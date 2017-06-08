using System.IO;

namespace OculiService.Common.IO
{
  internal sealed class DriveInfoAdapter : DriveInfoBase
  {
    private DriveInfo driveInfo;

    public override long AvailableFreeSpace
    {
      get
      {
        return this.driveInfo.AvailableFreeSpace;
      }
    }

    public override string DriveFormat
    {
      get
      {
        return this.driveInfo.DriveFormat;
      }
    }

    public override DriveType DriveType
    {
      get
      {
        return this.driveInfo.DriveType;
      }
    }

    public override bool IsReady
    {
      get
      {
        return this.driveInfo.IsReady;
      }
    }

    public override string Name
    {
      get
      {
        return this.driveInfo.Name;
      }
    }

    public override DirectoryInfoBase RootDirectory
    {
      get
      {
        return (DirectoryInfoBase) new DirectoryInfoAdapter(this.driveInfo.RootDirectory);
      }
    }

    public override long TotalFreeSpace
    {
      get
      {
        return this.driveInfo.TotalFreeSpace;
      }
    }

    public override long TotalSize
    {
      get
      {
        return this.driveInfo.TotalSize;
      }
    }

    public override string VolumeLabel
    {
      get
      {
        return this.driveInfo.VolumeLabel;
      }
      set
      {
        this.driveInfo.VolumeLabel = value;
      }
    }

    public DriveInfoAdapter(DriveInfo driveInfo)
    {
      this.driveInfo = driveInfo;
    }

    public override string ToString()
    {
      return this.driveInfo.ToString();
    }
  }
}
