using System.IO;

namespace OculiService.Common.IO
{
  public abstract class DriveInfoBase
  {
    public abstract long AvailableFreeSpace { get; }

    public abstract string DriveFormat { get; }

    public abstract DriveType DriveType { get; }

    public abstract bool IsReady { get; }

    public abstract string Name { get; }

    public abstract DirectoryInfoBase RootDirectory { get; }

    public abstract long TotalFreeSpace { get; }

    public abstract long TotalSize { get; }

    public abstract string VolumeLabel { get; set; }
  }
}
