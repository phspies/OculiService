using OculiService.Common.Logging;
using OculiService.Core.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;

public class SystemStateReader
{
    private const long ONE_MB = 1048576;
    private const ulong HKEY_LOCAL_MACHINE = 2147483650;

    public static SystemState GetSystemState(string serverName, string userName, string password, ILogger logger)
    {
        return SystemStateReader.GetSystemState(WMIUtils.ConnectToServer(serverName, userName, password), serverName, userName, password, logger);
    }

    public static SystemState GetSystemState(ManagementScope scope, string serverName, string userName, string password, ILogger logger)
    {
        try
        {
            ManagementScope serverDefaultPath = WMIUtils.ConnectToServerDefaultPath(serverName, userName, password);
            return SystemStateReader.GetSystemState(scope, serverDefaultPath, serverName, logger);
        }
        catch (Exception ex)
        {
            logger.Verbose("Exception thrown getting the system state for server " + serverName + ". Exception: " + ex.Message, "SystemState");
            throw;
        }
    }

    public static SystemState GetSystemState(ManagementScope scope, ManagementScope defaultScope, string serverName, ILogger logger)
    {
        SystemState systemState = new SystemState();
        try
        {
            systemState.RAM = CUtils.RoundUp(WMIUtils.GetRAM(scope) / 1048576L);
            systemState.CPUs = WMIUtils.GetNumberOfCPUs(scope);
            WMIUtils.GetOperatingSystemInfo(scope, ref systemState.OsInfo, ref systemState.SystemPath, ref systemState.SystemVolume);
            try
            {
                string str = CUtils.PathToUNC(serverName, systemState.SystemPath) + "\\System32\\hal.dll";
                if (File.Exists(str))
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(str);
                    systemState.HalInternalName = versionInfo.InternalName;
                    systemState.HalVersion = versionInfo.FileVersion;
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Verbose("Could not get Hal information. Exception: " + ex.Message, "SystemState");
                throw;
            }
            try
            {
                systemState.ProgramFilesPath = WMIUtils.GetRemoteRegistryValueString(defaultScope, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion", "ProgramFilesDir");
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Verbose("Could not get \"Program Files\" path. Exception: " + ex.Message, "SystemState");
                throw;
            }
            try
            {
                systemState.NetworkAdapters = SystemStateReader.GetNetworkAdapterList(scope);
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Verbose("Could not get the logical network adpater info from " + serverName + ". Exception: " + ex.Message, "SystemState");
                throw;
            }
            try
            {
                systemState.Volumes = SystemStateReader.GetVolumes(scope);
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Verbose("Could not get the volume info from " + serverName + ". Exception: " + ex.Message, "SystemState");
                throw;
            }
        }
        catch (Exception ex)
        {
            if (logger != null)
                logger.Verbose("Exception thrown getting the system state for server " + serverName + ". Exception: " + ex.Message, "SystemState");
            throw;
        }
        return systemState;
    }

    public static ServerNicInfo[] GetNetworkAdapterList(ManagementScope scope)
    {
        Dictionary<string, ServerNicInfo> dictionary = new Dictionary<string, ServerNicInfo>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
        ObjectQuery query = new ObjectQuery("Select * from Win32_NetworkAdapterConfiguration where IPEnabled = TRUE");
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query))
        {
            using (managementObjectSearcher.Get())
            {
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    ServerNicInfo serverNicInfo = new ServerNicInfo();
                    serverNicInfo.IPAddresses = (string[])managementObject["IPAddress"];
                    serverNicInfo.IPMasks = (string[])managementObject["IPSubnet"];
                    serverNicInfo.IPGateways = managementObject["DefaultIPGateway"] != null ? (string[])managementObject["DefaultIPGateway"] : (string[])null;
                    serverNicInfo.DNSAddrs = managementObject["DNSServerSearchOrder"] != null ? (string[])managementObject["DNSServerSearchOrder"] : (string[])null;
                    serverNicInfo.DNSDomain = (string)managementObject.Properties["DNSDomain"].Value;
                    serverNicInfo.TcpIpServiceUuid = (string)managementObject["SettingID"];
                    serverNicInfo.Index = (int)(uint)managementObject["Index"];
                    serverNicInfo.PNPInstanceId = WMIUtils.GetPNPInstanceId(scope, serverNicInfo.Index);
                    serverNicInfo.DHCPEnabled = (bool)managementObject["DHCPEnabled"];
                    ManagementObject firstElement = WMIUtils.GetFirstElement(managementObject.GetRelated("Win32_NetworkAdapter"));
                    serverNicInfo.FriendlyName = (string)firstElement["NetConnectionID"];
                    if (!string.IsNullOrEmpty(serverNicInfo.PNPInstanceId) && !dictionary.ContainsKey(serverNicInfo.TcpIpServiceUuid))
                        dictionary.Add(serverNicInfo.TcpIpServiceUuid, serverNicInfo);
                }
            }
        }
        return CUtils.CollectionToArray<ServerNicInfo>((ICollection<ServerNicInfo>)dictionary.Values);
    }

    public static ServerNicInfo[] GetVMBusNetworkAdapterList(ManagementScope scope)
    {
        Dictionary<string, ServerNicInfo> dictionary = new Dictionary<string, ServerNicInfo>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
        Dictionary<string, string> idsToPnpDeviceId = WMIUtils.GetVMBUSDeviceIdsToPNPDeviceId(scope);
        ObjectQuery query = new ObjectQuery("Select * from Win32_NetworkAdapterConfiguration where IPEnabled = TRUE");
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query))
        {
            using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
            {
                foreach (ManagementObject managementObject in objectCollection)
                {
                    string empty = string.Empty;
                    object obj = managementObject["Index"];
                    idsToPnpDeviceId.TryGetValue(obj.ToString(), out empty);
                    if (!string.IsNullOrEmpty(empty))
                    {
                        ServerNicInfo serverNicInfo = new ServerNicInfo();
                        serverNicInfo.Index = (int)(uint)obj;
                        serverNicInfo.PNPInstanceId = empty;
                        serverNicInfo.TcpIpServiceUuid = (string)managementObject["SettingID"];
                        if (!dictionary.ContainsKey(serverNicInfo.TcpIpServiceUuid))
                        {
                            serverNicInfo.DNSDomain = (string)managementObject.Properties["DNSDomain"].Value;
                            serverNicInfo.IPAddresses = (string[])managementObject["IPAddress"];
                            serverNicInfo.IPMasks = (string[])managementObject["IPSubnet"];
                            serverNicInfo.IPGateways = managementObject["DefaultIPGateway"] != null ? (string[])managementObject["DefaultIPGateway"] : (string[])null;
                            serverNicInfo.DNSAddrs = managementObject["DNSServerSearchOrder"] != null ? (string[])managementObject["DNSServerSearchOrder"] : (string[])null;
                            serverNicInfo.DHCPEnabled = (bool)managementObject["DHCPEnabled"];
                            ManagementObject firstElement = WMIUtils.GetFirstElement(managementObject.GetRelated("Win32_NetworkAdapter"));
                            serverNicInfo.FriendlyName = (string)firstElement["NetConnectionID"];
                            dictionary.Add(serverNicInfo.TcpIpServiceUuid, serverNicInfo);
                        }
                    }
                }
            }
        }
        return CUtils.CollectionToArray<ServerNicInfo>((ICollection<ServerNicInfo>)dictionary.Values);
    }

    public static VolumeInfo[] GetVolumes(ManagementScope scope)
    {
        ObjectQuery query = new ObjectQuery("Select * from Win32_LogicalDisk");
        List<VolumeInfo> volumeInfoList = new List<VolumeInfo>();
        OperatingSystemInfo OsInfo = (OperatingSystemInfo)null;
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        WMIUtils.GetOperatingSystemInfo(scope, ref OsInfo, ref empty1, ref empty2);
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query))
        {
            using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
            {
                foreach (ManagementObject managementObject in objectCollection)
                {
                    try
                    {
                        uint num1 = (uint)managementObject["DriveType"];
                        if ((int)num1 == 3)
                        {
                            string str1 = managementObject["Name"].ToString();
                            string str2 = managementObject["FileSystem"].ToString();
                            if (!string.IsNullOrEmpty(str1))
                            {
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    VolumeInfo volumeInfo1 = new VolumeInfo();
                                    volumeInfo1.DriveLetter = str1.Replace(":", "");
                                    volumeInfo1.Format = str2;
                                    volumeInfo1.Label = managementObject["VolumeName"].ToString();
                                    volumeInfo1.DiskSizeMB = (long)((ulong)managementObject["Size"] / 1048576UL);
                                    volumeInfo1.FreeSpaceMB = (long)((ulong)managementObject["FreeSpace"] / 1048576UL);
                                    volumeInfo1.DriveType = (int)num1;
                                    int num2 = empty2.StartsWith(str1) ? 1 : 0;
                                    volumeInfo1.IsSystemVolume = num2 != 0;
                                    VolumeInfo volumeInfo2 = volumeInfo1;
                                    volumeInfo2.UsedSpaceMB = volumeInfo2.DiskSizeMB - volumeInfo2.FreeSpaceMB;
                                    volumeInfoList.Add(volumeInfo2);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
        return volumeInfoList.ToArray();
    }

    public static string GetDriveLetterForNonSystemVolume(ManagementScope scope)
    {
        string empty = string.Empty;
        VolumeInfo[] volumes = SystemStateReader.GetVolumes(scope);
        return volumes.Length <= 1 ? volumes[0].DriveLetter : volumes.ToList<VolumeInfo>().Find((Predicate<VolumeInfo>)(x => !x.IsSystemVolume)).DriveLetter;
    }

    public static IEnumerable<string> GetDriveLetters(ManagementScope scope)
    {
        ManagementObjectCollection logicalDisks = WMIUtils.Query(scope, "select * from Win32_logicaldisk");
        try
        {
            foreach (ManagementObject managementObject1 in logicalDisks)
            {
                ManagementObject managementObject = managementObject1;
                try
                {
                    if (managementObject1["Name"] != null)
                        yield return (string)managementObject1["Name"];
                }
                finally
                {
                    if (managementObject != null)
                        managementObject.Dispose();
                }
                managementObject = (ManagementObject)null;
            }
        }
        finally
        {
            if (logicalDisks != null)
                logicalDisks.Dispose();
        }
        logicalDisks = (ManagementObjectCollection)null;
    }
}
