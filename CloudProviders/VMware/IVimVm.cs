using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimVm : IVimManagedItem
  {
    string Uuid { get; }

    VmProperties VMProperties { get; }

    string GuestOS { get; }

    string ResourcePoolName { get; }

    VirtualMachinePowerState PowerState { get; }

    VmProperties GetCommonProperties();

    void GetCommonProperties(Dictionary<string, object> vmProperties);

    VmProperties GetCommonProperties(ManagedObjectAndProperties[] managedObjects);

    VmConfiguration GetConfiguration();

    int GetNumOfPCIDevices();

    IVimTask[] GetRecentTasks();

    IVimResourcePool GetResourcePool();

    bool IsPoweredOn();

    bool IsSuspended();

    void AddScsiCtrl(int[] busNumbers, ScsiControllerType scsiControllerType, VimClientlContext ctx);

    ScsiControllerInfo[] GetScsiControllersInfo();

    IVimDatastore[] GetDatastoresAndProperties(ManagedObjectAndProperties[] data);

    IVimDatastore[] GetDatastoresAndProperties();

    IVimDatastore GetPrimaryDatastore(IVimDatastore[] datastores, string rawVmxPath);

    IVimDatastore GetPrimaryDatastore();

    string ReplaceDatastoreName(IVimDatastore[] datastores, string file);

    IVimHost GetHost(ManagedObjectAndProperties[] data);

    IVimHost GetHostAndProperties();

    IVimHost GetHostWithoutProperties();

    ManagedObjectAndProperties[] GetManagedObjectsAndProperties();

    VmxConfiguration CreateSourceVmdkMappings(VmxConfiguration vmxCgf, IVimDatastore[] datastores, IVimHost host, GetBaseVmdksDelegate getBaseVmdks);

    void CreateVirtualDisks(VmDiskInfo[] diskInfos, VimClientlContext ctx);

    VmdkProperties[] GetActiveDiskFilesLayout();

    void AddVirtualDisks(VmDiskInfo[] diskInfos, VimClientlContext ctx);

    Dictionary<string, string> GetVirtualDiskLabels();

    Dictionary<string, long> GetVirtualDiskSize();

    VmdkProperties[] GetVMDKInfo();

    VmdkProperties[] GetVMDKInfo(IVimDatastore[] datastores);

    bool IsVmdkChanged(ref VmxConfiguration srcCfg, IVimDatastore[] datastores, IVimHost host, GetBaseVmdksDelegate getBaseVmdks);

    void RemoveVirtualDisks(string[] names, VimClientlContext ctx);

    void RemoveVirtualDisksByFilename(string[] filenames, VimClientlContext ctx);

    void RemoveVirtualDisks(VmdkProperties[] VmdkProps, VimClientlContext ctx);

    void RemoveAllVirtualDisks(RemoveDisk removeDisk, VimClientlContext ctx);

    void AttachVirtualDisks(IVimVm vm, VimClientlContext ctx);

    void DetachVirtualDisks(IVimVm vm, VimClientlContext ctx);

    string GetRawVmxPath();

    string GetVmxFullName();

    string GetVmxFullName(IVimDatastore datastore, string rawVmxPath, string datastorePath);

    string GetVmxPath(IVimDatastore datastore, string rawVmxPath, string datastorePath);

    VirtualEthernetCard[] GetEthernetCards(IVimNetwork network);

    Dictionary<string, VirtualEthernetCard> GetEthernetCards();

    bool IsNicChanged(FailoverConfig failoverConfig);

    void SetNicConnectivity(bool enableConnectivity, VimClientlContext ctx);

    void SetConfiguration(FailoverConfig failoverConfig, VimClientlContext ctx);

    void SetConfiguration(FailoverConfig failoverConfig, bool disableAllNics, VimClientlContext ctx);

    IVimNetwork[] GetNetworksAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties);

    IVimNetwork[] GetNetworksAndProperties();

    void SetNicVirtualSwitches(string[] virtualSwitchNames, VimClientlContext ctx);

    void CloneVm(string targetVmName, IVimHost targetEsx, IVimDatastore targetDatastore, VimClientlContext ctx);

    void CloneVm(string targetVmName, IVimHost targetEsx, IVimDatastore targetDatastore, IVimResourcePool targetResourcePool, string customizationSpecName, string[] ipAddresses, VimClientlContext ctx);

    void MigrateVm(IVimHost targetHost);

    void PowerOff(VimClientlContext ctx);

    void PowerOn(VimClientlContext ctx);

    void RebootGuestOS(VimClientlContext ctx);

    void ShutdownGuestOS(VimClientlContext ctx);

    void CreateSnapshot(string snapName, string description, VimClientlContext ctx, bool bIncludeMemory, bool bQuiesceFileSystem);

    void CreateSnapshot(string snapName, string description, VimClientlContext ctx);

    bool HasUserDefinedSnapshots();

    void AssertSnapshots(string[] expectedSnapshotNames);

    IVimSnapshot GetSnapshot(string snapName);

    SnapshotsSummary GetSnapshotsSummary(string vmsdFile);

    bool HasSnapshotableDisks(VmdkProperties[] vmdkInfos);

    void RemoveAllSnapshots(VimClientlContext ctx);

    void RemoveSnapshot(string snapName, VimClientlContext ctx);

    void RemoveSnapshot(ManagedObjectReference mor, VimClientlContext ctx);

    void RevertToLastSnapshot(VimClientlContext ctx);

    void RevertToSnapshot(string snapName, VimClientlContext ctx);

    void TryRemoveAllSnapshots();

    VirtualMachineToolsStatus GetVMToolsStatus();

    void MountVMTools();

    void UnmountVMTools();

    void UpgradeVMTools(VimClientlContext ctx);

    VirtualMachineConfigSpec GetCompatibleConfigSpec(string targetDatastore, string replicaDisplayName);

    void Reconfigure(string op, VirtualMachineConfigSpec spec, VimClientlContext ctx);
  }
}
