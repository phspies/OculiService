using OculiService.Common;
using Oculi.Contract;
using OculiService.Core.Contract;
using OculiService.CloudProviders.Contract;
using System;

namespace Oculi.Jobs.Context
{
    public interface ITaskInfoWrapper
    {
        string HostName { get; }
        string HostDnsName { get; }
        HostUriBuilder ServerVimHostUriBuilder { get; }
        string ESXHostName { get; }
        VirtualNetworkInterfaceInfo[] NetworkInterfaceInfo { get; set; }
        VMInfo VmInfo { get; }
        string VMUUID { get; }
        OculiVolumeOptions[] Volumes { get; }
        OperatingSystemInfo OsInfo { get; }
        OculiVolumeOptions[] VolumeOptions { get; }
        VirtualSwitchMapping[] VirtualSwitchMapping { get; }
        string RunOnceAtStartup { get; set; }
        OculiVolumePersistedState[] VolumePersistedState { get; set; }
        string VmName { get; set; }
        Guid VmUuid { get; set; }
        string GuestOS { get; }
        int NumberOfCPUs { get; }
        int NumberOfCoresPerProcessor { get; }
        long RamSizeMB { get; }
        OculiInternalVirtualNetworkInterfaceInfo[] Nics { get; set; }
        DateTime StartTime { get; set; }
        DateTime StopTime { get; set; }
        string Name { get; }
        string Hypervisor { get; }
        string VmVersion { get; set; }
        void SetState(string lowLevelState);
    }
}
