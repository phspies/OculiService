using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace OculiService.Common.Diagnostics
{
  public class ProcessFactory : IProcessFactory
  {
    public IProcess CreateProcess(ProcessStartInfo startInfo)
    {
      Invariant.ArgumentNotNull((object) startInfo, "startInfo");
      if (startInfo.UseShellExecute || string.IsNullOrEmpty(startInfo.UserName))
        return (IProcess) new ProcessWrapper(Process.Start(startInfo));
      if (string.IsNullOrEmpty(startInfo.FileName))
        throw new InvalidOperationException("A file name must be provided for creating a process.");
      IntPtr zero = IntPtr.Zero;
      IntPtr num = startInfo.Password != null ? Marshal.SecureStringToCoTaskMemUnicode(startInfo.Password) : Marshal.StringToCoTaskMemUni(string.Empty);
      try
      {
        SafeFileHandle phToken;
        if (NativeMethods.LogonUser(startInfo.UserName, startInfo.Domain, num, 2, 0, out phToken) == 0)
          throw new Win32Exception(Marshal.GetLastWin32Error());
        int dwCreationFlags = 0;
        if (startInfo.CreateNoWindow)
          dwCreationFlags |= 134217728;
        IntPtr handle1 = IntPtr.Zero;
        GCHandle gcHandle = new GCHandle();
        if (startInfo.EnvironmentVariables != null)
        {
          bool unicode = false;
          if (Environment.OSVersion.Platform == PlatformID.Win32NT)
          {
            dwCreationFlags |= 1024;
            unicode = true;
          }
          gcHandle = GCHandle.Alloc((object) this.CreateEnvironmentBlock(startInfo.EnvironmentVariables, unicode), GCHandleType.Pinned);
          handle1 = gcHandle.AddrOfPinnedObject();
        }
        string lpCommandLine = this.BuildCommandLine(startInfo.FileName, startInfo.Arguments);
        string lpCurrentDirectory = string.IsNullOrEmpty(startInfo.WorkingDirectory) ? Environment.CurrentDirectory : startInfo.WorkingDirectory;
        NativeMethods.STARTUPINFO lpStartupInfo = new NativeMethods.STARTUPINFO();
        NativeMethods.PROCESS_INFORMATION lpProcessInformation = new NativeMethods.PROCESS_INFORMATION();
        try
        {
          if (NativeMethods.CreateProcessAsUser((SafeHandle) phToken, (string) null, lpCommandLine, IntPtr.Zero, IntPtr.Zero, false, dwCreationFlags, new HandleRef((object) null, handle1), lpCurrentDirectory, lpStartupInfo, lpProcessInformation) == 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        finally
        {
          phToken.Close();
          lpStartupInfo.Dispose();
          if (gcHandle.IsAllocated)
            gcHandle.Free();
        }
        Process processById = Process.GetProcessById(lpProcessInformation.dwProcessId);
        IntPtr handle2 = processById.Handle;
        NativeMethods.CloseHandle(lpProcessInformation.hProcess);
        NativeMethods.CloseHandle(lpProcessInformation.hThread);
        return (IProcess) new ProcessWrapper(processById);
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.ZeroFreeCoTaskMemUnicode(num);
      }
    }

    private string BuildCommandLine(string executableFileName, string arguments)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = executableFileName.Trim();
      int num = !str.StartsWith("\"", StringComparison.Ordinal) ? 0 : (str.EndsWith("\"", StringComparison.Ordinal) ? 1 : 0);
      if (num == 0)
        stringBuilder.Append("\"");
      stringBuilder.Append(str);
      if (num == 0)
        stringBuilder.Append("\"");
      if (!string.IsNullOrEmpty(arguments))
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(arguments);
      }
      return stringBuilder.ToString();
    }

    private byte[] CreateEnvironmentBlock(StringDictionary environmentVariables, bool unicode)
    {
      string[] keys = new string[environmentVariables.Count];
      environmentVariables.Keys.CopyTo((Array) keys, 0);
      string[] items = new string[environmentVariables.Count];
      environmentVariables.Values.CopyTo((Array) items, 0);
      Array.Sort<string, string>(keys, items, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < environmentVariables.Count; ++index)
      {
        stringBuilder.Append(keys[index]);
        stringBuilder.Append('=');
        stringBuilder.Append(items[index]);
        stringBuilder.Append(char.MinValue);
      }
      stringBuilder.Append(char.MinValue);
      byte[] numArray = !unicode ? Encoding.Default.GetBytes(stringBuilder.ToString()) : Encoding.Unicode.GetBytes(stringBuilder.ToString());
      if (numArray.Length > (int) ushort.MaxValue)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The environment block used to start a process cannot be longer than 65535 bytes.  Your environment block is {0} bytes long.  Remove some environment variables and try again.", new object[1]{ (object) numArray.Length }));
      return numArray;
    }
  }
}
