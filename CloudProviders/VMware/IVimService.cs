using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimService
  {
    DateTime CurrentTime { get; }

    ManagedObjectReference FileManager { get; }

    ManagedObjectReference VirtualDiskManager { get; }

    string FullName { get; }

    bool IsVirtualCenter { get; }

    ILogger Logger { get; }

    ManagedObjectReference PropertyCollector { get; }

    IVimFolderOutsideDC RootFolder { get; }

    VimService Service { get; }

    string ApiVersion { get; }

    List<IVimDatastore> GetAllDatastores();

    IVimDatastore GetDatastoreByName(string name);

    IVimDatastore GetDatastoreByUrl(string url);

    Dictionary<string, IVimHost> GetAllHostsDict();

    ObjectContent[] GetAllHostsObjectContents();

    IVimHost GetHost(string name, bool retrieveCommonProperties);

    IVimHost GetHost(string name);

    IVimHost[] GetHosts(IVimDatastore[] datastores);

    IVimHost GetHostManagedItem(ManagedObjectReference managedObject);

    Dictionary<string, IVimVm> GetAllVMsDictWithName();

    Dictionary<string, IVimVm> GetAllVMsDictWithUuid();

    void UnregisterAndDestroyVm(IVimVm vm, VimClientlContext ctx);

    void UnregisterVm(IVimVm vm);

    IVimVm GetVm(ManagedObjectReference managedObject);

    IVimVm GetVmOrVmTemplate(string name);

    Dictionary<string, object> PropSetToDictionary(DynamicProperty[] dynamicProperties);

    ObjectContent[] RetrieveProperties(PropertyFilterSpec[] pfSpec);

    ObjectContent[] getObjectContents(ManagedObjectReference[] managedObjects, string[] properties);

    Dictionary<ManagedObjectReference, Dictionary<string, object>> GetProperties(ManagedObjectReference[] managedObjects, string[] properties);

    Dictionary<ManagedObjectReference, Dictionary<string, object>> GetProperties(IVimManagedItem[] items, string[] properties);

    ManagedObjectReference[] VCManagedItemsToMors(IVimManagedItem[] items);

    void Heartbeat();

    void LogOff();

    void Logon();

    void Shutdown();

    int VC_CPU_Load();

    IVimDatacenter[] GetDatacenters();

    IVimDatacenter GetDatacenter(ManagedObjectReference managedObject);

    IVimHost SearchHostByDnsName(string dnsName, bool retrieveCommonProperties);

    IVimHost SearchHostByIP(string ip, bool retrieveCommonProperties);

    IVimHost SearchHostByUuid(string uuid, bool retrieveCommonProperties);

    IVimVm SearchVmByDnsName(string dnsName);

    IVimVm SearchVmByDnsName(string dnsName, bool retrieveCommonProperties);

    IVimVm SearchVmByUuid(string uuid, bool retrieveCommonProperties);

    IVimVm SearchVmByUuid(string uuid);

    List<string> SearchDatastoreSubFolder(string esxHost, string folderName, VimClientlContext ctx);

    Dictionary<string, InventoryNode> GetVmInventory();

    Dictionary<string, InventoryNode> GetHostInventory();

    InventoryNode GetRootFolderOfInventory(Dictionary<string, InventoryNode> inventory);

    IVimFolderOutsideDC GetFolderOutsideDC(ManagedObjectReference managedObject);

    IVimFolderInsideDC GetFolderInsideDC(ManagedObjectReference managedObject);

    CustomizationSpecItem GetCustomizationSpec(string name);
  }
}
