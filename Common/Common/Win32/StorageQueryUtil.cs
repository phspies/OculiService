using System;using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OculiService.Common.Win32
{
  public static class StorageQueryUtil
  {
    private const uint FILE_SHARE_READ = 1;
    private const uint FILE_SHARE_WRITE = 2;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 128;
    private const uint IOCTL_STORAGE_QUERY_PROPERTY = 2954240;

    [DllImport("kernel32.dll")]
    private static extern int CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint utBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

    public static int GetPhysicalSectorSize(string volume)
    {
      IntPtr num1 = IntPtr.Zero;
      IntPtr num2 = IntPtr.Zero;
      IntPtr num3 = IntPtr.Zero;
      try
      {
        num1 = StorageQueryUtil.CreateFile(string.Format("\\\\.\\{0}", (object) volume), 0U, 3U, IntPtr.Zero, 3U, 128U, IntPtr.Zero);
        StorageQueryUtil.STORAGE_PROPERTY_QUERY structure = new StorageQueryUtil.STORAGE_PROPERTY_QUERY() { QueryType = StorageQueryUtil.STORAGE_QUERY_TYPE.PropertyStandardQuery, PropertyId = StorageQueryUtil.STORAGE_PROPERTY_ID.StorageAccessAlignmentProperty };
        num2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (StorageQueryUtil.STORAGE_PROPERTY_QUERY)));
        Marshal.StructureToPtr<StorageQueryUtil.STORAGE_PROPERTY_QUERY>(structure, num2, true);
        int cb = Marshal.SizeOf(typeof (StorageQueryUtil.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR));
        num3 = Marshal.AllocHGlobal(cb);
        uint lpBytesReturned;
        if (StorageQueryUtil.DeviceIoControl(num1, 2954240U, num2, (uint) Marshal.SizeOf<StorageQueryUtil.STORAGE_PROPERTY_QUERY>(structure), num3, (uint) cb, out lpBytesReturned, IntPtr.Zero) == 0)
          throw new Win32Exception(Marshal.GetLastWin32Error());
        return (int) ((StorageQueryUtil.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR) Marshal.PtrToStructure(num3, typeof (StorageQueryUtil.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR))).BytesPerPhysicalSector;
      }
      finally
      {
        Marshal.FreeHGlobal(num3);
        Marshal.FreeHGlobal(num2);
        StorageQueryUtil.CloseHandle(num1);
      }
    }

    private enum STORAGE_PROPERTY_ID
    {
      StorageDeviceProperty,
      StorageAdapterProperty,
      StorageDeviceIdProperty,
      StorageDeviceUniqueIdProperty,
      StorageDeviceWriteCacheProperty,
      StorageMiniportProperty,
      StorageAccessAlignmentProperty,
      StorageDeviceSeekPenaltyProperty,
      StorageDeviceTrimProperty,
      StorageDeviceWriteAggregationProperty,
    }

    private enum STORAGE_QUERY_TYPE
    {
      PropertyStandardQuery,
      PropertyExistsQuery,
      PropertyMaskQuery,
      PropertyQueryMaxDefined,
    }

    private struct STORAGE_PROPERTY_QUERY
    {
      internal StorageQueryUtil.STORAGE_PROPERTY_ID PropertyId;
      internal StorageQueryUtil.STORAGE_QUERY_TYPE QueryType;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
      internal byte[] AdditonalParameters;
    }

    private struct STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR
    {
      internal uint Version;
      internal uint Size;
      internal uint BytesPerCacheLine;
      internal uint BytesOffsetForCacheAlignment;
      internal uint BytesPerLogicalSector;
      internal uint BytesPerPhysicalSector;
      internal uint BytesOffsetForSectorAlignment;
    }
  }
}
