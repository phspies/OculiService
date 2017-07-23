using System.Runtime.InteropServices;

namespace Common_Util.Win32API
{
  public struct LUID_AND_ATTRIBUTES
  {
    public LUID Luid;
    [MarshalAs(UnmanagedType.U4)]
    public PrivilegeAttributes Attributes;
  }
}
