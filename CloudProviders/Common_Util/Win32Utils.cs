using Common_Util.Properties;
using OculiService.Common.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;

public class Win32Utils
{
    public static readonly object HelperOSLock = new object();
    private const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 256;
    private const uint FORMAT_MESSAGE_IGNORE_INSERTS = 512;
    private const uint FORMAT_MESSAGE_FROM_SYSTEM = 4096;
    private const int MAX_PATH = 260;
    private const int MAX_ALTERNATE = 14;
    private const uint IO_REPARSE_TAG_SYMLINK = 2684354572;
    private const uint GENERIC_READ = 2147483648;
    private const uint GENERIC_WRITE = 1073741824;
    private const int FILE_SHARE_READ = 1;
    private const int FILE_SHARE_WRITE = 2;
    private const int OPEN_EXISTING = 3;
    private const int FILE_FLAG_BACKUP_SEMANTICS = 33554432;
    private const int FILE_FLAG_OPEN_REPARSE_POINT = 2097152;
    private const int INVALID_HANDLE_VALUE = -1;
    private const int FSCTL_GET_REPARSE_POINT = 589992;
    private const int MAXIMUM_REPARSE_DATA_BUFFER_SIZE = 16384;
    private const int FSCTL_LOCK_VOLUME = 589848;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] Win32Utils.MEMORYSTATUSEX lpBuffer);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FlushFileBuffers(IntPtr hFile);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, string[] Arguments);

    [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2W", CharSet = CharSet.Unicode)]
    public static extern int WNetAddConnection2(ref Win32Utils.NETRESOURCE netResource, [MarshalAs(UnmanagedType.LPWStr)] string password, [MarshalAs(UnmanagedType.LPWStr)] string username, [MarshalAs(UnmanagedType.U4)] int flags);

    [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2W", CharSet = CharSet.Unicode)]
    public static extern int WNetCancelConnection2([MarshalAs(UnmanagedType.LPWStr)] string lpName, [MarshalAs(UnmanagedType.U4)] int dwFlags, [MarshalAs(UnmanagedType.Bool)] bool bForce);

    [DllImport("kernel32.dll")]
    public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

    [DllImport("kernel32.dll", EntryPoint = "FindFirstFileW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr FindFirstFile(string lpFileName, out Win32Utils.WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll")]
    private static extern int CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    private static extern int DeviceIoControl(IntPtr hDevice, int dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int utBufferSize, out int lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

    public static bool GetAvailPhysicalMemoryInMB(ref int sizeInMB)
    {
        Win32Utils.MEMORYSTATUSEX lpBuffer = new Win32Utils.MEMORYSTATUSEX();
        if (!Win32Utils.GlobalMemoryStatusEx(lpBuffer))
            return false;
        ulong ullAvailPhys = lpBuffer.ullAvailPhys;
        sizeInMB = (int)(ullAvailPhys / 1024UL) / 1024;
        return true;
    }

    public static object GetRemoteRegistryValue(string server, string username, string password, string KeyName, string name)
    {
        object obj = (object)null;
        string format = Win32Utils.NetAddConnection(server, username, password);
        try
        {
            if (!string.IsNullOrEmpty(format))
                throw new ApplicationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, format, new object[0]));
            RegistryKey registryKey;
            try
            {
                registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server).OpenSubKey(KeyName);
            }
            catch (Exception ex)
            {
                registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server).OpenSubKey(KeyName);
            }
            if (registryKey != null)
                obj = registryKey.GetValue(name);
        }
        finally
        {
            Win32Utils.NetCancelConnection(server);
        }
        return obj;
    }

    public static string NetAddConnection(string server, string username, string password)
    {
        Win32Utils.NETRESOURCE netResource = new Win32Utils.NETRESOURCE();
        netResource.dwDisplayType = 1;
        netResource.dwScope = 0;
        netResource.dwType = 0;
        netResource.dwUsage = 2;
        netResource.LocalName = "";
        netResource.RemoteName = IPHelper.GetWNetServerString(server);
        netResource.Provider = (string)null;
        string username1;
        string domain;
        CUtils.SplitUsernameAndDomain(username, out username1, out domain);
        username = !string.IsNullOrEmpty(domain) ? username1 + "@" + domain : server.Replace(':', '-') + "\\" + username1;
        int num = Win32Utils.WNetAddConnection2(ref netResource, password, username, 0);
        string str;
        switch (num)
        {
            case 0:
            case 1219:
            case 71:
                str = (string)null;
                break;
            default:
                str = Win32Utils.FormatWin32ErrorMessage((uint)num);
                break;
        }
        return str;
    }

    public static string FormatWin32ErrorMessage(uint error)
    {
        IntPtr zero = IntPtr.Zero;
        string str;
        if ((int)Win32Utils.FormatMessage(4864U, IntPtr.Zero, error, 0U, ref zero, 0U, (string[])null) == 0)
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.InvalidServer, new object[1]
            {
        (object) error
            });
        else
            str = Marshal.PtrToStringAnsi(zero);
        return str;
    }

    public static void NetCancelConnection(string server)
    {
        Win32Utils.WNetCancelConnection2(IPHelper.GetWNetServerString(server), 0, true);
    }

    public static string ValidateCredentials(string server, string username, string password)
    {
        try
        {
            return Win32Utils.NetAddConnection(server, username, password);
        }
        finally
        {
            Win32Utils.NetCancelConnection(server);
        }
    }

    public static string ValidateCredentials(string server, NetworkCredential credentials)
    {
        string fullUserName = CUtils.GetFullUserName(credentials);
        return Win32Utils.ValidateCredentials(server, fullUserName, credentials.Password);
    }

    public static bool IsDotNetFramework2PointOInstalled(string server, string username, string password)
    {
        return Win32Utils.IsMicrosoftComponentInstalled(server, username, password, ".NET Framework", "2.0");
    }

    public static bool IsMicrosoftComponentInstalled(string server, string username, string password, string componentFriendlyName, string version)
    {
        bool flag = false;
        string message = Win32Utils.NetAddConnection(server, username, password);
        try
        {
            if (!string.IsNullOrEmpty(message))
                throw new ApplicationException(message);
            RegistryKey registryKey1;
            try
            {
                registryKey1 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server).OpenSubKey("SOFTWARE\\Microsoft\\Active Setup\\Installed Components");
            }
            catch (IOException ex)
            {
                registryKey1 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server).OpenSubKey("SOFTWARE\\Microsoft\\Active Setup\\Installed Components");
            }
            foreach (string subKeyName in registryKey1.GetSubKeyNames())
            {
                RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName);
                string str1 = (string)registryKey2.GetValue((string)null);
                if (!string.IsNullOrEmpty(str1) && str1.IndexOf(componentFriendlyName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    string str2 = ((string)registryKey2.GetValue("Version")).Trim();
                    if (!string.IsNullOrEmpty(str2) && str2.Replace(",", ".").StartsWith(version))
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }
        finally
        {
            Win32Utils.NetCancelConnection(server);
        }
        return flag;
    }

    public static bool IsInstalledDTCompatible(string server, string username, string password, string installPath)
    {
        FileVersionInfo versionInfo;
        try
        {
            versionInfo = FileVersionInfo.GetVersionInfo(installPath + "\\setup.exe");
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException("File not found: " + ex.Message);
        }
        if (versionInfo == null)
            return false;
        object remoteRegistryValue = Win32Utils.GetRemoteRegistryValue(server, username, password, "SOFTWARE\\Nsi Software\\Double-Take\\CurrentVersion", "VersionInfo");
        if (remoteRegistryValue == null)
            return false;
        string str = ((string)remoteRegistryValue).Trim().TrimEnd(".".ToCharArray());
        return versionInfo.FileVersion.Trim().TrimEnd(".".ToCharArray()) == str;
    }

    public static int GetLocalOculiPort()
    {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\NSI Software\\Double-Take\\CurrentVersion");
        string name1 = "Port";
        if (registryKey.GetValue(name1) == null)
            throw new KeyNotFoundException("The key (Port) is not found in the registry.");
        string name2 = "Port";
        return (int)registryKey.GetValue(name2);
    }

    public static int GetOculiPort(ManagementScope defaultScope)
    {
        return WMIUtils.GetRemoteRegistryValueInt(defaultScope, "SOFTWARE\\NSI Software\\Double-Take\\CurrentVersion", "Port");
    }

    public static bool IsSymLink(string fullPath)
    {
        Win32Utils.WIN32_FIND_DATA lpFindFileData;
        if (Win32Utils.FindFirstFile(fullPath, out lpFindFileData).ToInt32() == -1)
            throw new FileNotFoundException();
        return (lpFindFileData.dwFileAttributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint & ((long)lpFindFileData.dwReserved0 & 2684354572L) == 2684354572L;
    }

    public static bool GetDirectoryFreeDiskSpace(string directory, out ulong freeSpace)
    {
        bool flag = false;
        freeSpace = 0UL;
        ulong lpFreeBytesAvailable = 0;
        int startIndex;
        for (ulong lpTotalNumberOfBytes = 0; Win32Utils.GetDiskFreeSpaceEx(directory, out lpFreeBytesAvailable, out lpTotalNumberOfBytes, out freeSpace) == 0; directory = directory.Remove(startIndex))
        {
            startIndex = directory.LastIndexOf('\\');
            if (startIndex == -1)
                goto label_5;
        }
        flag = true;
        label_5:
        return flag;
    }

    public static bool GetDirectoryFreeDiskSpaceUnc(string uncPath, out ulong freeSpace)
    {
        freeSpace = 0UL;
        ulong lpFreeBytesAvailable = 0;
        ulong lpTotalNumberOfBytes = 0;
        return Win32Utils.GetDiskFreeSpaceEx(uncPath, out lpFreeBytesAvailable, out lpTotalNumberOfBytes, out freeSpace) != 0;
    }

    public static bool IsSANPolicyOnline(ManagementScope defaultScope)
    {
        bool flag = false;
        if (WMIUtils.GetRemoteRegistryValueInt(defaultScope, "SYSTEM\\CurrentControlSet\\Services\\partmgr\\Parameters", "SanPolicy") == 1)
            flag = true;
        return flag;
    }

    public static bool LockVolume(string volumeName, ILogger logger)
    {
        bool flag = false;
        IntPtr file = Win32Utils.CreateFile(volumeName, 3221225472U, 3U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
        if (file.ToInt32() != -1)
        {
            int lpBytesReturned;
            if (Win32Utils.DeviceIoControl(file, 589848, IntPtr.Zero, 0, IntPtr.Zero, 0, out lpBytesReturned, IntPtr.Zero) != 0)
                flag = true;
            else
                logger.Warning("error locking volume: " + volumeName + " error: " + (object)Marshal.GetHRForLastWin32Error());
            Win32Utils.CloseHandle(file);
        }
        return flag;
    }

    public static bool FlushVolume(string volumeName, ILogger logger)
    {
        bool flag = false;
        IntPtr file = Win32Utils.CreateFile(volumeName, 3221225472U, 3U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
        if (file.ToInt32() != -1)
        {
            if (!Win32Utils.FlushFileBuffers(file))
            {
                logger.Warning("Failed to flush this mount point: " + volumeName);
                logger.Warning("Flush error: " + (object)Marshal.GetLastWin32Error());
            }
            else
            {
                logger.Information("Successfully flushed mount point: " + volumeName);
                flag = true;
            }
        }
        else
            logger.Warning("Failed to open a valid handle to " + volumeName + " Could not flush!!");
        return flag;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            this.dwLength = (uint)Marshal.SizeOf(typeof(Win32Utils.MEMORYSTATUSEX));
        }
    }

    public struct NETRESOURCE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwScope;
        [MarshalAs(UnmanagedType.U4)]
        public int dwType;
        [MarshalAs(UnmanagedType.U4)]
        public int dwDisplayType;
        [MarshalAs(UnmanagedType.U4)]
        public int dwUsage;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string LocalName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string RemoteName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Provider;
    }

    private struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WIN32_FIND_DATA
    {
        public FileAttributes dwFileAttributes;
        public Win32Utils.FILETIME ftCreationTime;
        public Win32Utils.FILETIME ftLastAccessTime;
        public Win32Utils.FILETIME ftLastWriteTime;
        public int nFileSizeHigh;
        public int nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MOUNT_POINT_REPARSE_BUFFER
    {
        public uint ReparseTag;
        public short ReparseDataLength;
        public short Reserved;
        public short SubstituteNameOffset;
        public short SubstituteNameLength;
        public short PrintNameOffset;
        public short PrintNameLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16368)]
        public byte[] PathBuffer;
    }
}
