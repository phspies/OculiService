using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

public class SafeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public SafeHandle(IntPtr tokenHandle)
      : base(true)
    {
        this.SetHandle(tokenHandle);
    }

    protected override bool ReleaseHandle()
    {
        return (uint)SafeHandle.CloseHandle(this.handle) > 0U;
    }

    [DllImport("kernel32.dll")]
    private static extern int CloseHandle(IntPtr hObject);
}
