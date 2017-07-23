using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimHost : IVimManagedItem
  {
    ServerProperties Properties { get; set; }

    ServerProperties GetCommonProperties(ManagedObjectAndProperties[] managedObjects);

    void GetCommonProperties(Dictionary<string, object> hostProperties);

    ServerProperties GetCommonProperties();

    HostConfiguration GetConfiguration();

    IVimDatacenter GetDatacenterAndProperties();

    IVimHost[] GetHosts();

    ManagedObjectAndProperties[] GetMangedObjectsAndProperties();

    bool UserHasPermissions(out int userRole);

    IVimDatastore GetDatastoreByName(string name);

    IVimDatastore GetDatastoreByUrl(string url);

    string GetDatastorePathByUrl(string url);

    IVimDatastore[] GetDatastoresAndProperties();

    IVimDatastore[] GetDatastoresAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties);

    List<string> SearchDatastoreSubFolder(string folderName, VimClientlContext ctx);

    long GetDatastoreMaxVmdkSizeMB(string url);

    long GetDatastoreMaxVmdkSizeMB(IVimDatastore ds);

    ManagedObjectReference GetComputeResource();

    IVimResourcePool[] GetAllResourcePools();

    IVimResourcePool GetDefaultResourcePool();

    IVimResourcePool GetResourcePoolByName(string resPoolName);

    void MoveVmToResourcePool(IVimVm vm, string resPoolName);

    Dictionary<string, string[]> GetVirtualSwitch();

    Dictionary<string, string> GetDistributedVirtualPortgroups();

    Dictionary<string, string> GetDistributedVirtualSwitchUuids();

    IVimNetwork[] GetNetworks();

    IVimNetwork[] GetNetworksAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties);

    long GetMemory();

    short GetNumberCPU();

    short GetNumberCpuThreads();

    short GetNumberCpuPackages();

    string GetUuid();

    IVimVm GetVm(string name);

    IVimVm GetVmByUuid(string uuid);

    IVimVm[] GetVmsAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties);

    IVimVm[] GetVmsAndProperties();

    IVimVm[] GetVmTemplatesAndProperties();

    IVimVm RegisterVm(string dsPath, string resPoolName, VimClientlContext ctx);

    IVimVm SearchVmByUuid(string uuid);

    void UnregisterVm(IVimVm vm);

    IVimVm GetRecentlyCreatedVm(ManagedObjectReference task);

    string ContainsVmName(string vmName);

    IVimVm CreateVm(VmCreationInfo vmCreationInfo, VimClientlContext ctx);

    IVimVm CreateVm(VirtualMachineConfigSpec configSpec, VimClientlContext ctx);

    IVimVm CreateVmWithNetworkMapping(VirtualMachineConfigSpec configSpec, Dictionary<string, string> networkMap, VimClientlContext ctx);
  }
}
