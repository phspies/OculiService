using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Common_Util
{
  public class PartitionInformation : IEquatable<PartitionInformation>
  {
    public ushort Access;
    public ushort Availability;
    public ulong BlockSize;
    public bool Bootable;
    public bool BootPartition;
    public string Caption;
    public uint ConfigManagerErrorCode;
    public string Description;
    public uint DiskIndex;
    public string DeviceID;
    public uint HiddenSectors;
    public uint Index;
    public DateTime InstallDate;
    public string Name;
    public ulong NumberOfBlocks;
    public string PNPDeviceID;
    public bool PrimaryPartition;
    public string Purpose;
    public ulong Size;
    public ulong StartingOffset;
    public string Status;
    public string Type;

    public static Dictionary<uint, PartitionInformation> GetDiskPartitionInformation(DiskInformation diskInfo)
    {
      return PartitionInformation.GetDiskPartitionInformation(diskInfo.Index);
    }

    public static Dictionary<uint, PartitionInformation> GetDiskPartitionInformation(uint diskIndex)
    {
      using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ObjectQuery(string.Format("SELECT * FROM Win32_DiskPartition WHERE DiskIndex = {0}", (object) diskIndex))))
      {
        Trace.Assert(managementObjectSearcher != null, "wbemSearcher is null");
        using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
        {
          Dictionary<uint, PartitionInformation> dictionary = new Dictionary<uint, PartitionInformation>();
          foreach (ManagementObject managementObject in objectCollection)
          {
            if (managementObject != null)
            {
              using (managementObject)
              {
                uint maxValue = uint.MaxValue;
                if (managementObject["Index"] != null)
                  maxValue = (uint) managementObject["Index"];
                if (!dictionary.Keys.Contains<uint>(maxValue))
                {
                  PartitionInformation partitionInformation1 = new PartitionInformation();
                  partitionInformation1.Access = (ushort) (managementObject["Access"] != null ? (int) (ushort) managementObject["Access"] : (int) ushort.MaxValue);
                  partitionInformation1.Availability = (ushort) (managementObject["Availability"] != null ? (int) (ushort) managementObject["Availability"] : (int) ushort.MaxValue);
                  int num1 = managementObject["Bootable"] != null ? ((bool) managementObject["Bootable"] ? 1 : 0) : 0;
                  partitionInformation1.Bootable = num1 != 0;
                  int num2 = managementObject["BootPartition"] != null ? ((bool) managementObject["BootPartition"] ? 1 : 0) : 0;
                  partitionInformation1.BootPartition = num2 != 0;
                  long num3 = managementObject["BlockSize"] != null ? (long) (ulong) managementObject["BlockSize"] : -1L;
                  partitionInformation1.BlockSize = (ulong) num3;
                  string str1 = managementObject["Caption"] != null ? (string) managementObject["Caption"] : string.Empty;
                  partitionInformation1.Caption = str1;
                  int num4 = managementObject["ConfigManagerErrorCode"] != null ? (int) (uint) managementObject["ConfigManagerErrorCode"] : -1;
                  partitionInformation1.ConfigManagerErrorCode = (uint) num4;
                  string str2 = managementObject["Description"] != null ? (string) managementObject["Description"] : string.Empty;
                  partitionInformation1.Description = str2;
                  string str3 = managementObject["DeviceID"] != null ? (string) managementObject["DeviceID"] : string.Empty;
                  partitionInformation1.DeviceID = str3;
                  int num5 = managementObject["DiskIndex"] != null ? (int) (uint) managementObject["DiskIndex"] : -1;
                  partitionInformation1.DiskIndex = (uint) num5;
                  int num6 = managementObject["HiddenSectors"] != null ? (int) (uint) managementObject["HiddenSectors"] : -1;
                  partitionInformation1.HiddenSectors = (uint) num6;
                  int num7 = managementObject["Index"] != null ? (int) (uint) managementObject["Index"] : -1;
                  partitionInformation1.Index = (uint) num7;
                  DateTime dateTime = managementObject["InstallDate"] != null ? ManagementDateTimeConverter.ToDateTime((string) managementObject["InstallDate"]) : DateTime.MaxValue;
                  partitionInformation1.InstallDate = dateTime;
                  string str4 = managementObject["Name"] != null ? (string) managementObject["Name"] : string.Empty;
                  partitionInformation1.Name = str4;
                  long num8 = managementObject["NumberOfBlocks"] != null ? (long) (ulong) managementObject["NumberOfBlocks"] : -1L;
                  partitionInformation1.NumberOfBlocks = (ulong) num8;
                  string str5 = managementObject["PNPDeviceID"] != null ? (string) managementObject["PNPDeviceID"] : string.Empty;
                  partitionInformation1.PNPDeviceID = str5;
                  int num9 = managementObject["PrimaryPartition"] != null ? ((bool) managementObject["PrimaryPartition"] ? 1 : 0) : 0;
                  partitionInformation1.PrimaryPartition = num9 != 0;
                  string str6 = managementObject["Purpose"] != null ? (string) managementObject["Purpose"] : string.Empty;
                  partitionInformation1.Purpose = str6;
                  long num10 = managementObject["Size"] != null ? (long) (ulong) managementObject["Size"] : -1L;
                  partitionInformation1.Size = (ulong) num10;
                  long num11 = managementObject["StartingOffset"] != null ? (long) (ulong) managementObject["StartingOffset"] : -1L;
                  partitionInformation1.StartingOffset = (ulong) num11;
                  string str7 = managementObject["Status"] != null ? (string) managementObject["Status"] : string.Empty;
                  partitionInformation1.Status = str7;
                  string str8 = managementObject["Type"] != null ? (string) managementObject["Type"] : string.Empty;
                  partitionInformation1.Type = str8;
                  PartitionInformation partitionInformation2 = partitionInformation1;
                  dictionary.Add(maxValue, partitionInformation2);
                }
              }
            }
          }
          return dictionary;
        }
      }
    }

    public bool Equals(PartitionInformation other)
    {
      if ((int) this.Index == (int) other.Index)
        return (int) this.DiskIndex == (int) other.DiskIndex;
      return false;
    }
  }
}
