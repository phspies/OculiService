using System;

namespace OculiService.CloudProviders.VMware
{
  public class NoSuchVmException : ApplicationException
  {
    public NoSuchVmException(string msg)
      : base(msg)
    {
    }
  }
}
