using System;using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OculiService.Common.IO
{
  internal sealed class DriveAdapter : DriveBase
  {
    public override DriveInfoBase GetInfo(string driveName)
    {
      return (DriveInfoBase) new DriveInfoAdapter(new DriveInfo(driveName));
    }

    public override DriveInfoBase[] GetDrives()
    {
      return (DriveInfoBase[]) ((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).Select<DriveInfo, DriveInfoAdapter>((Func<DriveInfo, DriveInfoAdapter>) (d => new DriveInfoAdapter(d))).ToArray<DriveInfoAdapter>();
    }
  }
}
