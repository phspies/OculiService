// Decompiled with JetBrains decompiler
// Type: Common_Util.Registry.GrantBackupAndRestorePrivilege
// Assembly: Oculi.Virtualization.Common_Util, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 729775AE-A0F3-4545-B89A-1AD01077DBC4
// Assembly location: C:\Downloads\Double-Take\Service\Oculi.Virtualization.Common_Util.dll

using Common_Util.Win32API;
using System;
using System.Runtime.InteropServices;

namespace Common_Util.Registry
{
  public class GrantBackupAndRestorePrivilege : IDisposable
  {
    private IntPtr _ProcessHandle;
    private IntPtr _TokenHandle;

    public GrantBackupAndRestorePrivilege()
    {
      this._ProcessHandle = Win32Interface.OpenProcess(ProcessAccessFlags.SetInformation | ProcessAccessFlags.QueryInformation, false, Win32Interface.GetCurrentProcessId());
      Win32Interface.OpenProcessToken(this._ProcessHandle, TokenAccessFlags.Query | TokenAccessFlags.AdjustPrivileges, out this._TokenHandle);
      TOKEN_PRIVILEGES_SIMPLE NewState = new TOKEN_PRIVILEGES_SIMPLE();
      TOKEN_PRIVILEGES_SIMPLE PreviousState = new TOKEN_PRIVILEGES_SIMPLE();
      LUID lpLuid;
      if (!Win32Interface.LookupPrivilegeValue("", "SeRestorePrivilege", out lpLuid))
        return;
      NewState.PrivilegeCount = 1U;
      NewState.Luid = lpLuid;
      NewState.Attributes = PrivilegeAttributes.Enabled;
      int ReturnLength1 = Marshal.SizeOf<TOKEN_PRIVILEGES_SIMPLE>(PreviousState);
      Win32Interface.AdjustTokenPrivileges(this._TokenHandle, false, ref NewState, Marshal.SizeOf<TOKEN_PRIVILEGES_SIMPLE>(NewState), ref PreviousState, ref ReturnLength1);
      Win32Interface.LookupPrivilegeValue("", "SeBackupPrivilege", out lpLuid);
      Win32Interface.LookupPrivilegeValue("", "SeSecurityPrivilege", out lpLuid);
      Win32Interface.LookupPrivilegeValue("", "SeTakeOwnershipPrivilege", out lpLuid);
      NewState.PrivilegeCount = 1U;
      NewState.Luid = lpLuid;
      NewState.Attributes = PrivilegeAttributes.Enabled;
      int ReturnLength2 = Marshal.SizeOf<TOKEN_PRIVILEGES_SIMPLE>(PreviousState);
      Win32Interface.AdjustTokenPrivileges(this._TokenHandle, false, ref NewState, Marshal.SizeOf<TOKEN_PRIVILEGES_SIMPLE>(NewState), ref PreviousState, ref ReturnLength2);
    }

    public void Dispose()
    {
      Win32Interface.CloseHandle(this._TokenHandle);
      Win32Interface.CloseHandle(this._ProcessHandle);
    }
  }
}
