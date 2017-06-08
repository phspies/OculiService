using System.Runtime.InteropServices;

namespace OculiService.Common.Net
{
  internal struct _SERVER_INFO_100
  {
    internal int sv100_platform_id;
    [MarshalAs(UnmanagedType.LPWStr)]
    internal string sv100_name;
  }
}
