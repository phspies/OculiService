using System.Collections.Generic;

namespace OculiService.Common.IO
{
  public abstract class NetworkShareBase
  {
    public abstract IEnumerable<NetworkShareInfo> GetShares(bool includeSecurity = false);

    public abstract void CreateFileShare(NetworkShareInfo share);

    public abstract void DeleteFileShare(string name);
  }
}
