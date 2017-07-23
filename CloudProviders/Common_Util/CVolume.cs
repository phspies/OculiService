using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

public class CVolume
{
    public const int FMIFS_HARDDISK = 12;
    private string _volumeId;

    public string FileSystem
    {
        get
        {
            StringBuilder FileSystemNameBuffer = new StringBuilder(256);
            uint VolumeSerialNumber;
            uint MaximumComponentLength;
            uint FileSystemFlags;
            CVolume.GetVolumeInformation(this._volumeId, (StringBuilder)null, 0, out VolumeSerialNumber, out MaximumComponentLength, out FileSystemFlags, FileSystemNameBuffer, FileSystemNameBuffer.Capacity);
            return FileSystemNameBuffer.ToString();
        }
    }

    public CVolume(string volumeId)
    {
        if (!volumeId.EndsWith("\\"))
            volumeId += "\\";
        this._volumeId = volumeId;
    }

    public static CVolume CreateCVolumeFromVolumePathname(string volumePathName)
    {
        StringBuilder lpszVolumeName = new StringBuilder((int)short.MaxValue);
        if (!CVolume.GetVolumeNameForVolumeMountPoint(volumePathName, lpszVolumeName, (uint)short.MaxValue))
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        return new CVolume(lpszVolumeName.ToString());
    }

    public static bool UnmountDrive(string mountPoint)
    {
        if (!Directory.Exists(mountPoint))
            return false;
        if (!mountPoint.EndsWith("\\"))
            mountPoint += "\\";
        return CVolume.DeleteVolumeMountPoint(mountPoint);
    }

    public bool FormatDrive(bool quickFormat, string format, string label, long diskSizeMB)
    {
        string volumeId = this._volumeId.TrimEnd("\\".ToCharArray());
        CVolume.FormatCallBackDelegate callBackDelegate1 = new CVolume.FormatCallBackDelegate(CVolume.formatCallBack);
        int mediaFlag = 12;
        string fsType = format;
        string label1 = label;
        int quickFormat1 = quickFormat ? 1 : 0;
        int clusterSize = 0;
        CVolume.FormatCallBackDelegate callBackDelegate2 = callBackDelegate1;
        CVolume.FormatEx(volumeId, mediaFlag, fsType, label1, quickFormat1, clusterSize, callBackDelegate2);
        return string.Compare(format, this.FileSystem, true, CultureInfo.InvariantCulture) == 0;
    }

    public bool MountDrive(string mountPoint)
    {
        if (!Directory.Exists(mountPoint))
            Directory.CreateDirectory(mountPoint);
        if (!mountPoint.EndsWith("\\"))
            mountPoint += "\\";
        return CVolume.SetVolumeMountPoint(mountPoint, this._volumeId);
    }

    public List<string> GetMountPointsForVolume()
    {
        if (string.IsNullOrEmpty(this._volumeId))
            return new List<string>();
        string volumePathNames = new string(char.MinValue, 4096);
        int lpcchReturnLength = 0;
        bool namesForVolumeNameW = CVolume.GetVolumePathNamesForVolumeNameW(this._volumeId, volumePathNames, volumePathNames.Length, ref lpcchReturnLength);
        if (!namesForVolumeNameW && Marshal.GetLastWin32Error() == 234)
        {
            volumePathNames = new string(char.MinValue, lpcchReturnLength);
            namesForVolumeNameW = CVolume.GetVolumePathNamesForVolumeNameW(this._volumeId, volumePathNames, volumePathNames.Length, ref lpcchReturnLength);
        }
        if (!namesForVolumeNameW)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "GetVolumePathNamesForVolumeNameW failed to obtain volume path names");
        List<string> stringList = new List<string>();
        foreach (string str in volumePathNames.Split(new char[1]))
        {
            if (str.Length > 0)
                stringList.Add(str);
        }
        return stringList;
    }

    public static List<string> GetAllVolumeIDs()
    {
        List<string> stringList = new List<string>();
        foreach (ManagementObject managementObject in WMIUtils.Query(WMIUtils.ConnectToServer("localhost", "", (string)null), "Select DeviceID From Win32_Volume"))
            stringList.Add((string)managementObject["DeviceID"]);
        return stringList;
    }

    public static VolumeInfo GetVolumeInfo(string volumeId)
    {
        VolumeInfo volumeInfo = new VolumeInfo();
        if (!volumeId.EndsWith("\\"))
            volumeId += "\\";
        ManagementObject managementObject = WMIUtils.QueryFirst(WMIUtils.ConnectToServer("localhost", (string)null, (string)null), "Select * From Win32_Volume Where DeviceID='" + volumeId.Replace("\\", "\\\\") + "'");
        volumeInfo.DiskSizeMB = (long)(ulong)managementObject["Capacity"] / 1048576L;
        volumeInfo.FreeSpaceMB = (long)(ulong)managementObject["Freespace"] / 1048576L;
        volumeInfo.UsedSpaceMB = volumeInfo.DiskSizeMB - volumeInfo.FreeSpaceMB;
        volumeInfo.Format = (string)managementObject["FileSystem"];
        volumeInfo.Label = (string)managementObject["Label"];
        volumeInfo.DriveType = (int)(uint)managementObject["DriveType"];
        return volumeInfo;
    }

    public static bool DoesVolumeExist(uint serialNumber)
    {
        return 0 < WMIUtils.Query(WMIUtils.ConnectToServer("localhost", (string)null, (string)null), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "SELECT * FROM Win32_Volume WHERE SerialNumber='{0}'", new object[1] { (object)serialNumber })).Count;
    }

    public static bool FindVolumeSymboliclinkBySignature(byte[] signature, ref string volumeOut)
    {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\MountedDevices"))
        {
            foreach (string valueName in registryKey.GetValueNames())
            {
                if (CVolume.SameSequence((byte[])registryKey.GetValue(valueName, (object)new byte[0]), signature) && valueName.Length == 48 && valueName.StartsWith("\\??\\"))
                {
                    volumeOut = valueName;
                    return true;
                }
            }
        }
        return false;
    }

    public static bool FindVolumeDosDeviceNameBySignature(byte[] signature, ref string volumeOut)
    {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\MountedDevices"))
        {
            foreach (string valueName in registryKey.GetValueNames())
            {
                if (CVolume.SameSequence((byte[])registryKey.GetValue(valueName, (object)new byte[0]), signature) && valueName.Contains("DosDevices"))
                {
                    volumeOut = valueName;
                    return true;
                }
            }
        }
        return false;
    }

    public static string GetVolumeNameForVolumeMountPoint(string volumeMountPoint)
    {
        StringBuilder lpszVolumeName = new StringBuilder((int)short.MaxValue);
        if (!CVolume.GetVolumeNameForVolumeMountPoint(volumeMountPoint, lpszVolumeName, (uint)short.MaxValue))
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        return lpszVolumeName.ToString();
    }

    private static bool SameSequence(byte[] sig1, byte[] sig2)
    {
        if (sig1 == null || sig2 == null)
            return false;
        return ((IEnumerable<byte>)sig1).SequenceEqual<byte>((IEnumerable<byte>)sig2);
    }

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool GetVolumeNameForVolumeMountPoint(string volumeName, StringBuilder uniqueVolumeName, int uniqueNameBufferCapacity);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetVolumeMountPoint(string lpszVolumeMountPoint, string lpszVolumeName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool DeleteVolumeMountPoint(string lpszVolumeMountPoint);

    [DllImport("fmifs.dll", CharSet = CharSet.Auto)]
    public static extern void FormatEx(string volumeId, int mediaFlag, string fsType, string label, int quickFormat, int clusterSize, CVolume.FormatCallBackDelegate callBackDelegate);

    public static int formatCallBack(CVolume.CallbackCommand callBackCommand, int subActionCommand, IntPtr action)
    {
        return 1;
    }

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool GetVolumeInformation(string RootPathName, StringBuilder VolumeNameBuffer, int VolumeNameSize, out uint VolumeSerialNumber, out uint MaximumComponentLength, out uint FileSystemFlags, StringBuilder FileSystemNameBuffer, int nFileSystemNameSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetVolumePathNamesForVolumeNameW([MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeName, [MarshalAs(UnmanagedType.LPWStr)] string volumePathNames, int cchBuferLength, ref int lpcchReturnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetVolumeNameForVolumeMountPoint(string lpszVolumeMountPoint, [Out] StringBuilder lpszVolumeName, uint cchBufferLength);

    public enum CallbackCommand
    {
        PROGRESS,
        DONEWITHSTRUCTURE,
        UNKNOWN2,
        UNKNOWN3,
        UNKNOWN4,
        UNKNOWN5,
        INSUFFICIENTRIGHTS,
        UNKNOWN7,
        DISKLOCKEDFORACCESS,
        UNKNOWN9,
        UNKNOWNA,
        DONE,
        UNKNOWNC,
        UNKNOWND,
        OUTPUT,
        STRUCTUREPROGRESS,
    }

    public delegate int FormatCallBackDelegate(CVolume.CallbackCommand callBackCommand, int subActionCommand, IntPtr action);
}
