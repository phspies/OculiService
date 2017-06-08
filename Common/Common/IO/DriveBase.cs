namespace OculiService.Common.IO
{
  public abstract class DriveBase
  {
    public abstract DriveInfoBase GetInfo(string driveName);

    public abstract DriveInfoBase[] GetDrives();
  }
}
