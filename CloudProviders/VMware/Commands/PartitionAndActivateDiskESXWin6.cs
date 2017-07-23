using Microsoft.Win32;
using Oculi.Commands.Interfaces;

namespace OculiService.CloudProviders.VMware
{
  public class PartitionAndActivateDiskESXWin6 : JobCommand, IJobCommandVolumeInfo
  {
    public PartitionAndActivateDiskESXWin6(JobContext context)
      : base(context)
    {
    }

    public bool Invoke(OculiVolumePersistedState volumeInfo)
    {
      volumeInfo.IsGpt = volumeInfo.DesiredSize > 2199023255552L;
      return this._RunDiskPart("select disk " + (object) volumeInfo.DriveIndex + "\nattributes disk clear readonly noerr\nonline disk noerr\nclean\nconvert dynamic\nconvert basic\n" + (volumeInfo.IsGpt ? "convert gpt\n" : "convert mbr\n") + "create partition primary\nselect partition 1\n" + (volumeInfo.IsSystemDrive ? "active\n" : string.Empty));
    }

    protected virtual bool _RunDiskPart(string script)
    {
      int timeoutMilliseconds = (int) Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Nsi Software\\Oculi\\CurrentVersion", "DiskPartHungTimeout", (object) 1800000);
      return CUtils.DiskPart(script, Win32Utils.HelperOSLock, this._Context.Logger, timeoutMilliseconds);
    }
  }
}
