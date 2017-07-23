using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Common_Util
{
  public class DiskInformation : IEquatable<DiskInformation>
  {
    public static DiskInformation.DiskInformationComparer Comparer = new DiskInformation.DiskInformationComparer();
    public ushort ScsiTargetId;
    public string DeviceID;
    public uint Index;
    public string PNPDeviceID;
    public uint Signature;

    public static Dictionary<uint, DiskInformation> GetCurrentDiskInformation()
    {
      using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_DiskDrive")))
      {
        Trace.Assert(managementObjectSearcher != null, "wbemSearcher is null");
        using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
        {
          Dictionary<uint, DiskInformation> dictionary = new Dictionary<uint, DiskInformation>();
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
                  dictionary.Add(maxValue, new DiskInformation()
                  {
                    Index = managementObject["Index"] != null ? (uint) managementObject["Index"] : uint.MaxValue,
                    ScsiTargetId = managementObject["SCSITargetId"] != null ? (ushort) managementObject["SCSITargetId"] : ushort.MaxValue,
                    DeviceID = managementObject["DeviceID"] != null ? (string) managementObject["DeviceID"] : string.Empty,
                    PNPDeviceID = managementObject["PNPDeviceID"] != null ? (string) managementObject["PNPDeviceID"] : string.Empty,
                    Signature = managementObject["Signature"] != null ? (uint) managementObject["Signature"] : uint.MaxValue
                  });
              }
            }
          }
          return dictionary;
        }
      }
    }

    public bool Equals(DiskInformation other)
    {
      return (int) this.Index == (int) other.Index;
    }

    public class DiskInformationComparer : IEqualityComparer<KeyValuePair<int, DiskInformation>>
    {
      public bool Equals(KeyValuePair<int, DiskInformation> x, KeyValuePair<int, DiskInformation> y)
      {
        return x.Key == y.Key;
      }

      public int GetHashCode(KeyValuePair<int, DiskInformation> obj)
      {
        return obj.Key;
      }
    }
  }
}
