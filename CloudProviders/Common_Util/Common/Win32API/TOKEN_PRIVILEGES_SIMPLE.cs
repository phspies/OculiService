using System.Runtime.InteropServices;

namespace Common_Util.Win32API
{
  public struct TOKEN_PRIVILEGES_SIMPLE
  {
    public uint PrivilegeCount;
    public LUID Luid;
    [MarshalAs(UnmanagedType.U4)]
    public PrivilegeAttributes Attributes;
  }
}
