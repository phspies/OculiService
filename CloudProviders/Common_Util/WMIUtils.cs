using OculiService.Core.Contract;
using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Threading;

public class WMIUtils
{
    private const ulong HKEY_LOCAL_MACHINE = 2147483650;

    public static ManagementScope ConnectToServer(string serverName, string userName, string password, string wmiNamespace)
    {
        ManagementScope managementScope;
        if (!IPHelper.IsLocalServer(serverName))
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Password = password;
            string username;
            string domain;
            CUtils.SplitUsernameAndDomain(userName, out username, out domain);
            options.Username = username;
            options.Authority = !string.IsNullOrEmpty(domain) ? "NTLMDOMAIN:" + domain : "NTLMDOMAIN:" + serverName;
            managementScope = new ManagementScope("\\\\" + serverName + wmiNamespace, options);
            managementScope.Connect();
        }
        else
        {
            managementScope = new ManagementScope(wmiNamespace);
            managementScope.Connect();
        }
        return managementScope;
    }

    public static ManagementScope ConnectToServer(string serverName, string userName, string password)
    {
        return WMIUtils.ConnectToServer(serverName, userName, password, "\\root\\cimv2");
    }

    public static ManagementScope ConnectToLocalServer()
    {
        return WMIUtils.ConnectToServer("localhost", "", "", "\\root\\cimv2");
    }

    public static ManagementScope ConnectToServer(string server, NetworkCredential credentials)
    {
        string userName = CUtils.CombinUsernameAndDomain(credentials.UserName, credentials.Domain);
        return WMIUtils.ConnectToServer(server, userName, credentials.Password);
    }

    public static ManagementScope ConnectToServerDefaultPath(string serverName, string userName, string password)
    {
        return WMIUtils.ConnectToServer(serverName, userName, password, "\\root\\default");
    }

    public static ManagementScope ConnectToServerDefaultPath(string server, NetworkCredential credentials)
    {
        string userName = CUtils.CombinUsernameAndDomain(credentials.UserName, credentials.Domain);
        return WMIUtils.ConnectToServerDefaultPath(server, userName, credentials.Password);
    }

    public static long GetRAM(ManagementScope scope)
    {
        long num = 0;
        ObjectQuery query = new ObjectQuery("Select TotalPhysicalMemory from WIN32_ComputerSystem");
        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher(scope, query).Get().GetEnumerator())
        {
            if (enumerator.MoveNext())
                num = (long)(ulong)enumerator.Current["TotalPhysicalMemory"];
        }
        return num;
    }

    public static int GetNumberOfCPUs(ManagementScope scope)
    {
        ManagementObject managementObject = WMIUtils.QueryFirst(scope, "Select * From Win32_ComputerSystem");
        object propertyValue = WMIUtils.GetPropertyValue((ManagementBaseObject)managementObject, "NumberOfLogicalProcessors");
        return propertyValue == null ? (int)(uint)WMIUtils.GetPropertyValue((ManagementBaseObject)managementObject, "NumberOfProcessors") : (int)(uint)propertyValue;
    }

    public static string GetBIOSUuid(ManagementScope scope)
    {
        string str = (string)null;
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery("Select * from Win32_ComputerSystemProduct")))
        {
            using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
            {
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objectCollection.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        str = (string)enumerator.Current["UUID"];
                }
            }
        }
        return str;
    }

    public static OperatingSystemArchitecture GetCPUArchitecture(ManagementScope scope, int major)
    {
        OperatingSystemArchitecture systemArchitecture = OperatingSystemArchitecture.x86;
        if (major == 5)
        {
            ObjectQuery query = new ObjectQuery("Select Architecture From WIN32_Processor");
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher(scope, query).Get().GetEnumerator())
            {
                if (enumerator.MoveNext())
                    systemArchitecture = (OperatingSystemArchitecture)(short)(ushort)enumerator.Current["Architecture"];
            }
        }
        else if (major == 6)
        {
            ObjectQuery query = new ObjectQuery("Select OSArchitecture From WIN32_OperatingSystem");
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher(scope, query).Get().GetEnumerator())
            {
                if (enumerator.MoveNext())
                    systemArchitecture = !enumerator.Current["OSArchitecture"].ToString().Contains("32") ? OperatingSystemArchitecture.x64 : OperatingSystemArchitecture.x86;
            }
        }
        return systemArchitecture;
    }

    public static OperatingSystemInfo GetOperatingSystemInfo(ManagementScope scope)
    {
        OperatingSystemInfo operatingSystemInfo = new OperatingSystemInfo();
        using (ManagementObject managementObject = WMIUtils.QueryFirst(scope, "Select * from Win32_OperatingSystem"))
        {
            string str = managementObject["Version"].ToString();
            if (!string.IsNullOrEmpty(str))
            {
                string[] strArray = str.Split(".".ToCharArray());
                operatingSystemInfo.VersionString = str;
                operatingSystemInfo.Version = new OperatingSystemVersion()
                {
                    Major = Convert.ToInt32(strArray[0]),
                    Minor = Convert.ToInt32(strArray[1]),
                    Build = Convert.ToInt32(managementObject["BuildNumber"])
                };
            }
            operatingSystemInfo.ServicePack = string.Format("{0}.{1}.0.0", (object)Convert.ToInt32(managementObject["ServicePackMajorVersion"]).ToString(), (object)Convert.ToInt32(managementObject["ServicePackMinorVersion"]).ToString());
            operatingSystemInfo.ProductType = (OperatingSystemProductType)(uint)managementObject["ProductType"];
            if (managementObject["OSProductSuite"] != null)
                operatingSystemInfo.ProductSuite = (int)(uint)managementObject["OSProductSuite"];
            operatingSystemInfo.Architecture = WMIUtils.GetCPUArchitecture(scope, operatingSystemInfo.Version.Major);
        }
        return operatingSystemInfo;
    }

    public static void GetOperatingSystemInfo(ManagementScope scope, ref OperatingSystemInfo OsInfo, ref string systemPath, ref string systemVolume)
    {
        OsInfo = WMIUtils.GetOperatingSystemInfo(scope);
        using (ManagementObject managementObject = WMIUtils.QueryFirst(scope, "Select * from Win32_OperatingSystem"))
        {
            systemPath = managementObject["SystemDirectory"].ToString().Replace("\\system32", "");
            systemVolume = systemPath.Substring(0, 2);
        }
    }

    public static string GetRemoteRegistryValueString(ManagementScope scope, string subKeyName, string valueName)
    {
        return WMIUtils.GetRemoteRegistryValue(scope, "GetStringValue", subKeyName, valueName).ToString();
    }

    public static int GetRemoteRegistryValueInt(ManagementScope scope, string subKeyName, string valueName)
    {
        return Convert.ToInt32(WMIUtils.GetRemoteRegistryValue(scope, "GetDWORDValue", subKeyName, valueName));
    }

    public static byte[] GetRemoteRegistryBinaryValue(ManagementScope scope, string subKeyName, string valueName)
    {
        return WMIUtils.GetRemoteRegistryValue(scope, "GetBinaryValue", subKeyName, valueName) as byte[];
    }

    private static object GetRemoteRegistryValue(ManagementScope scope, string methodName, string subKeyName, string valueName)
    {
        ManagementPath path = new ManagementPath("StdRegProv");
        ManagementClass managementClass = new ManagementClass(scope, path, (ObjectGetOptions)null);
        string methodName1 = methodName;
        ManagementBaseObject methodParameters = managementClass.GetMethodParameters(methodName1);
        methodParameters["hDefKey"] = (object)2147483650UL;
        methodParameters["sSubKeyName"] = (object)subKeyName;
        methodParameters["sValueName"] = (object)valueName;
        string methodName2 = methodName;
        ManagementBaseObject inParameters = methodParameters;
        object local = null;
        using (ManagementBaseObject managementBaseObject = managementClass.InvokeMethod(methodName2, inParameters, (InvokeMethodOptions)local))
        {
            int int32 = Convert.ToInt32(managementBaseObject["ReturnValue"]);
            if (int32 != 0)
                throw new OculiServiceException(int32, "Error getting registry value " + subKeyName + " from server " + scope.ToString() + ", error = " + int32.ToString());
            if (string.Compare(methodName, "GetStringValue", true) == 0)
                return managementBaseObject["sValue"];
            return managementBaseObject["uValue"];
        }
    }

    public static Dictionary<string, string> GetVMBUSDeviceIdsToPNPDeviceId(ManagementScope scope)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        try
        {
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery("Select * from Win32_NetworkAdapter WHERE PNPDeviceID Like \"%VMBUS%\"")))
            {
                using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
                {
                    foreach (ManagementObject managementObject in objectCollection)
                        dictionary.Add(managementObject["DeviceID"].ToString(), managementObject["PNPDeviceID"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
        }
        return dictionary;
    }

    public static string GetPNPInstanceId(ManagementScope scope, int deviceId)
    {
        string empty = string.Empty;
        try
        {
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery("Select * from Win32_NetworkAdapter Where DeviceID = " + deviceId.ToString())))
            {
                using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
                {
                    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objectCollection.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                            empty = enumerator.Current["PNPDeviceID"].ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }
        return empty;
    }

    public static bool RenameComputer(ManagementScope scope, string newName)
    {
        try
        {
            ManagementPath path = new ManagementPath("WIN32_ComputerSystem");
            ManagementClass managementClass = new ManagementClass(scope, path, (ObjectGetOptions)null);
            string methodName = "Rename";
            ManagementBaseObject methodParameters = managementClass.GetMethodParameters(methodName);
            methodParameters["Name"] = (object)newName;
            ManagementBaseObject managementBaseObject = (ManagementBaseObject)null;
            foreach (ManagementObject instance in managementClass.GetInstances())
                managementBaseObject = instance.InvokeMethod("Rename", methodParameters, (InvokeMethodOptions)null);
            return Convert.ToInt32(managementBaseObject.GetPropertyValue("ReturnValue")) == 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static void RebootComputer(ManagementScope scope)
    {
        try
        {
            ManagementPath path = new ManagementPath("WIN32_OperatingSystem");
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementClass(scope, path, (ObjectGetOptions)null).GetInstances().GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return;
                ((ManagementObject)enumerator.Current).InvokeMethod("Reboot", (object[])null);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static ProcessInfo GetProcessInfo(ManagementScope scope, string processName)
    {
        ProcessInfo processInfo = new ProcessInfo();
        processInfo.Name = processName;
        ObjectQuery query1 = new ObjectQuery("Select Name,PercentProcessorTime,Timestamp_Sys100NS From Win32_PerfRawData_PerfProc_Process Where Name='" + processName + "'");
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query1);
        ulong num1 = 0;
        ulong num2 = 0;
        ulong num3 = 0;
        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                ManagementObject current = (ManagementObject)enumerator.Current;
                num1 = (ulong)current["PercentProcessorTime"];
                if (current["Timestamp_Sys100NS"] != null)
                    num2 = (ulong)current["Timestamp_Sys100NS"];
            }
        }
        Thread.Sleep(1000);
        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher(scope, query1).Get().GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                ManagementObject current = (ManagementObject)enumerator.Current;
                ulong num4 = (ulong)current["PercentProcessorTime"];
                if (current["Timestamp_Sys100NS"] != null)
                    num3 = (ulong)current["Timestamp_Sys100NS"];
                processInfo.PercentCPU = new ulong?((num4 - num1) * 100UL / (num3 - num2));
            }
        }
        ObjectQuery query2 = new ObjectQuery("Select Name, CreationDate From WIN32_Process");
        foreach (ManagementObject managementObject in new ManagementObjectSearcher(scope, query2).Get())
        {
            if (((string)managementObject["Name"]).Split('.')[0] == processName)
            {
                string dmtfDate = managementObject["CreationDate"].ToString();
                processInfo.CreationDate = WMIUtils.ToDateTime(dmtfDate);
                break;
            }
        }
        return processInfo;
    }

    public static ProcessInfo GetProcessInfo(string server, string username, string password, string processName)
    {
        ProcessInfo processInfo = (ProcessInfo)null;
        try
        {
            processInfo = WMIUtils.GetProcessInfo(WMIUtils.ConnectToServer(server, username, password), processName);
        }
        catch (Exception ex)
        {
        }
        return processInfo;
    }

    private static DateTime ToDateTime(string dmtfDate)
    {
        int year = DateTime.Now.Year;
        int month = 1;
        int day = 1;
        int hour = 0;
        int minute = 0;
        int second = 0;
        int millisecond = 0;
        string str = dmtfDate;
        string empty = string.Empty;
        if (string.Empty == str || str == null || str.Length != 25)
            return DateTime.MinValue;
        string s1 = str.Substring(0, 4);
        if ("****" != s1)
            year = int.Parse(s1);
        string s2 = str.Substring(4, 2);
        if ("**" != s2)
            month = int.Parse(s2);
        string s3 = str.Substring(6, 2);
        if ("**" != s3)
            day = int.Parse(s3);
        string s4 = str.Substring(8, 2);
        if ("**" != s4)
            hour = int.Parse(s4);
        string s5 = str.Substring(10, 2);
        if ("**" != s5)
            minute = int.Parse(s5);
        string s6 = str.Substring(12, 2);
        if ("**" != s6)
            second = int.Parse(s6);
        string s7 = str.Substring(15, 3);
        if ("***" != s7)
            millisecond = int.Parse(s7);
        return new DateTime(year, month, day, hour, minute, second, millisecond);
    }

    public static bool IsDTRachetInstalled(ManagementScope scope)
    {
        using (ManagementObject managementObject = WMIUtils.QueryFirst(scope, "Select * From WIN32_SystemDriver Where Name='RepDrv'"))
            return managementObject != null;
    }

    public static void PrintProperties(ManagementBaseObject mbo)
    {
        foreach (PropertyData property in mbo.Properties)
        {
            if (property.Value is Array)
            {
                Console.Write("Name: {0}\nType: {1}\nValue: ", (object)property.Name, (object)property.Type);
                foreach (object obj in (Array)property.Value)
                    Console.Write("{0};", (object)obj.ToString());
                Console.WriteLine();
            }
            else
                Console.WriteLine("Name: {0}\nType: {1}\nValue: {2}", (object)property.Name, (object)property.Type, property.Value);
        }
    }

    public static void PrintProperties(ManagementObjectCollection moc)
    {
        foreach (ManagementObject managementObject in moc)
        {
            Console.WriteLine("///////////////////////////////////////////");
            WMIUtils.PrintProperties((ManagementBaseObject)managementObject);
        }
    }

    public static object GetPropertyValue(ManagementBaseObject mbo, string propertyName)
    {
        object obj = (object)null;
        try
        {
            obj = mbo.GetPropertyValue(propertyName);
        }
        catch (Exception ex)
        {
        }
        return obj;
    }

    public static ManagementObject GetFirstElement(ManagementObjectCollection moc)
    {
        if (moc == null)
            return (ManagementObject)null;
        using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = moc.GetEnumerator())
        {
            if (enumerator.MoveNext())
                return (ManagementObject)enumerator.Current;
            return (ManagementObject)null;
        }
    }

    public static ManagementObjectCollection Query(ManagementScope scope, string query)
    {
        ObjectQuery query1 = new ObjectQuery(query);
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, query1);
        try
        {
            return managementObjectSearcher.Get();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Query failed: " + query + ", error: " + ex.Message);
        }
        finally
        {
            managementObjectSearcher.Dispose();
        }
    }

    public static ManagementObject QueryFirst(ManagementScope scope, string query)
    {
        return WMIUtils.GetFirstElement(WMIUtils.Query(scope, query));
    }

    public static ManagementObject GetHDD(ManagementScope scope, HddLocation hddLocation)
    {
        string query = string.Format("Select * From Win32_DiskDrive Where SCSIBus={0} And SCSILogicalUnit={1} And SCSIPort={2} And SCSITargetId={3}", (object)hddLocation.Bus, (object)hddLocation.Lun, (object)hddLocation.Port, (object)hddLocation.Target);
        ManagementObject managementObject = WMIUtils.QueryFirst(scope, query);
        if (managementObject != null)
            return managementObject;
        throw new ApplicationException("GetHDD failed with " + hddLocation.ToString());
    }

    public static List<string> GetMSFailoverClusterNicsIDs()
    {
        List<string> stringList = new List<string>();
        using (ManagementObjectCollection objectCollection = WMIUtils.Query(WMIUtils.ConnectToServer("localhost", "", (string)null), "Select * from Win32_NetworkAdapterConfiguration Where IPEnabled=True"))
        {
            foreach (ManagementObject managementObject in objectCollection)
            {
                if (string.Compare((string)managementObject["ServiceName"], "Netft", true) == 0)
                {
                    string str = ((string)managementObject["SettingID"]).TrimStart("{".ToCharArray()).TrimEnd("}".ToCharArray());
                    stringList.Add(str);
                }
            }
        }
        return stringList;
    }

    public static string GetRemoteSystemDirectory(string serverName, NetworkCredential networkCredential)
    {
        return WMIUtils.GetSystemDirectory(WMIUtils.ConnectToServer(serverName, networkCredential.UserName, networkCredential.Password, "\\root\\cimv2"));
    }

    public static string GetSystemDirectory(ManagementScope cimv2Scope)
    {
        using (ManagementObject managementObject = WMIUtils.QueryFirst(cimv2Scope, "select * from win32_operatingsystem"))
            return (string)managementObject["SystemDirectory"];
    }

    public static void ComputerSystem(ManagementScope cimv2Scope, out string domain, out string domainRole)
    {
        using (ManagementObject managementObject = WMIUtils.QueryFirst(cimv2Scope, "select * from Win32_ComputerSystem"))
        {
            domain = (string)managementObject["Domain"];
            domainRole = managementObject["DomainRole"].ToString();
        }
    }

    public static byte[] GetRemoteBootVolumeSignature(ManagementScope defaultScope)
    {
        string valueName = "\\DosDevices\\" + WMIUtils.GetRemoteRegistryValueString(defaultScope, "Software\\Microsoft\\Windows NT\\CurrentVersion", "SystemRoot").Split('\\').GetValue(0);
        return WMIUtils.GetRemoteRegistryBinaryValue(defaultScope, "SYSTEM\\MountedDevices", valueName);
    }

    public static bool HasDynamicDisk(ManagementScope scope, List<string> volumes)
    {
        bool flag = false;
        foreach (string volume in volumes)
        {
            try
            {
                if (WMIUtils.StartsWith(WMIUtils.GetDiskSignature(scope, volume), new byte[8] { (byte)68, (byte)77, (byte)73, (byte)79, (byte)58, (byte)73, (byte)68, (byte)58 }))
                {
                    flag = true;
                    break;
                }
            }
            catch (Exception ex)
            {
            }
        }
        return flag;
    }

    private static byte[] GetDiskSignature(ManagementScope scope, string volume)
    {
        return WMIUtils.GetRemoteRegistryBinaryValue(scope, "SYSTEM\\MountedDevices", "\\DosDevices\\" + volume + ":");
    }

    private static bool StartsWith(byte[] o, byte[] sw)
    {
        if (o.Length < sw.Length)
            return false;
        for (int index = 0; index < sw.Length; ++index)
        {
            if ((int)o[index] != (int)sw[index])
                return false;
        }
        return true;
    }
}
