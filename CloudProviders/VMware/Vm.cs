using OculiService.CloudProviders.Oculi;
using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
    internal class Vm : VCManagedItem, IVimVm, IVimManagedItem
    {
        public static string[] VCProperties = new string[10] { "config.template", "config.uuid", "config.name", "config.guestFullName", "runtime.powerState", "config.guestId", "summary.guest.hostName", "config.hardware.numCPU", "config.hardware.memoryMB", "config.version" };
        private int _maxSnapshotDelta = 500;
        private ILogger _logger;
        private VmProperties _vmProperties;

        public VmProperties VMProperties
        {
            get
            {
                if (string.IsNullOrEmpty(this._vmProperties.Name))
                    this.GetCommonProperties();
                return this._vmProperties;
            }
        }

        public string ResourcePoolName
        {
            get
            {
                return this.GetResourcePool().Name;
            }
        }

        public string GuestOS
        {
            get
            {
                if (string.IsNullOrEmpty(this._vmProperties.GuestFullName))
                    this.GetCommonProperties(this.GetProperties(Vm.VCProperties));
                return this._vmProperties.GuestFullName;
            }
        }

        public VirtualMachinePowerState PowerState
        {
            get
            {
                if (string.IsNullOrEmpty(this._vmProperties.Uuid))
                    this.GetCommonProperties(this.GetProperties(Vm.VCProperties));
                return this._vmProperties.PowerState;
            }
        }

        public string Uuid
        {
            get
            {
                if (string.IsNullOrEmpty(this._vmProperties.Uuid))
                    this.GetCommonProperties(this.GetProperties(Vm.VCProperties));
                return this._vmProperties.Uuid;
            }
        }

        internal Vm(IVimService vimService, ManagedObjectReference managedObject) : base(vimService, managedObject)
        {
            this._logger = vimService.Logger;
        }

        internal Vm(IVimService vimService, string name) : base(vimService, (ManagedObjectReference)null)
        {
            this.Name = name;
            this._logger = vimService.Logger;
        }

        public VmProperties GetCommonProperties()
        {
            this.GetCommonProperties(this.GetProperties(Vm.VCProperties));
            return this._vmProperties;
        }

        public VmProperties GetCommonProperties(ManagedObjectAndProperties[] managedObjects)
        {
            foreach (ManagedObjectAndProperties managedObject in managedObjects)
            {
                if (managedObject.ManagedObject.Value == this.ManagedObject.Value)
                {
                    this.GetCommonProperties(managedObject.Properties);
                    break;
                }
            }
            return this._vmProperties;
        }

        public void GetCommonProperties(Dictionary<string, object> vmProperties)
        {
            if (vmProperties.ContainsKey("config.uuid"))
                this._vmProperties.Uuid = (string)vmProperties["config.uuid"];
            if (vmProperties.ContainsKey("config.name"))
                this._vmProperties.Name = (string)vmProperties["config.name"];
            if (vmProperties.ContainsKey("config.guestFullName"))
                this._vmProperties.GuestFullName = (string)vmProperties["config.guestFullName"];
            if (vmProperties.ContainsKey("runtime.powerState"))
                this._vmProperties.PowerState = (VirtualMachinePowerState)vmProperties["runtime.powerState"];
            if (vmProperties.ContainsKey("config.guestId"))
                this._vmProperties.GuestId = (string)vmProperties["config.guestId"];
            if (vmProperties.ContainsKey("config.version"))
                this._vmProperties.Version = (string)vmProperties["config.version"];
            if (vmProperties.ContainsKey("summary.guest.hostName"))
                this._vmProperties.HostName = (string)vmProperties["summary.guest.hostName"];
            if (vmProperties.ContainsKey("config.template"))
                this._vmProperties.IsTemplate = (bool)vmProperties["config.template"];
            if (vmProperties.ContainsKey("config.hardware.numCPU"))
                this._vmProperties.NumCPU = (int)vmProperties["config.hardware.numCPU"];
            if (vmProperties.ContainsKey("config.hardware.memoryMB"))
                this._vmProperties.MemoryMB = (int)vmProperties["config.hardware.memoryMB"];
            if (!string.IsNullOrEmpty(this._vmProperties.HostName) && this._vmProperties.HostName.EndsWith("."))
                this._vmProperties.HostName = this._vmProperties.HostName.TrimEnd(".".ToCharArray());
            this.Name = this._vmProperties.Name;
        }

        public IVimDatastore[] GetDatastoresAndProperties(ManagedObjectAndProperties[] data)
        {
            List<IVimDatastore> vimDatastoreList = new List<IVimDatastore>();
            foreach (ManagedObjectAndProperties objectAndProperties in data)
            {
                if (!(objectAndProperties.ManagedObject.type != "Datastore") && objectAndProperties.Properties != null)
                {
                    IVimDatastore vimDatastore = (IVimDatastore)new Datastore(this.VcService, objectAndProperties.ManagedObject);
                    vimDatastore.GetCommonProperties(objectAndProperties.Properties);
                    if (!string.IsNullOrEmpty(vimDatastore.DsProperties.RemoteId))
                        vimDatastoreList.Add(vimDatastore);
                }
            }
            return vimDatastoreList.ToArray();
        }

        public IVimDatastore[] GetDatastoresAndProperties()
        {
            return this.GetDatastoresAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "datastore", "Datastore", Datastore.VCProperties));
        }

        public string GetRawVmxPath()
        {
            string str = (string)null;
            Dictionary<string, object> properties = this.GetProperties(new string[1] { "summary.config.vmPathName" });
            if (properties != null && properties.Count > 0)
                str = (string)properties["summary.config.vmPathName"];
            return str;
        }

        public IVimDatastore GetPrimaryDatastore()
        {
            Dictionary<string, object> propertyValues;
            return this.GetPrimaryDatastore(this.GetDatastoresAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "datastore", "Datastore", Datastore.VCProperties, new string[1] { "summary.config.vmPathName" }, out propertyValues)), (string)propertyValues["summary.config.vmPathName"]);
        }

        public IVimDatastore GetPrimaryDatastore(IVimDatastore[] datastores, string rawVmxPath)
        {
            IVimDatastore vimDatastore = (IVimDatastore)null;
            if (datastores != null)
            {
                foreach (IVimDatastore datastore in datastores)
                {
                    if (rawVmxPath.Contains("[" + datastore.Name + "]"))
                    {
                        vimDatastore = datastore;
                        break;
                    }
                }
            }
            return vimDatastore;
        }

        public VmdkProperties[] GetVMDKInfo()
        {
            return this.GetVMDKInfo(this.GetDatastoresAndProperties());
        }

        public VmdkProperties[] GetVMDKInfo(IVimDatastore[] datastores)
        {
            Dictionary<string, object> properties = this.GetProperties(new string[1] { "config.hardware.device" });
            List<VmdkProperties> vmdkPropertiesList = new List<VmdkProperties>();
            List<VirtualSCSIController> virtualScsiControllerList = new List<VirtualSCSIController>();
            List<VirtualIDEController> virtualIdeControllerList = new List<VirtualIDEController>();
            string index1 = "config.hardware.device";
            foreach (VirtualDevice virtualDevice in (VirtualDevice[])properties[index1])
            {
                if (virtualDevice is VirtualDisk)
                {
                    VmdkProperties vmdkProperties = new VmdkProperties();
                    vmdkProperties.FileName = ((VirtualDeviceFileBackingInfo)virtualDevice.backing).fileName;
                    if (virtualDevice.backing is VirtualDiskFlatVer1BackingInfo)
                        vmdkProperties.DiskMode = ((VirtualDiskFlatVer1BackingInfo)virtualDevice.backing).diskMode;
                    else if (virtualDevice.backing is VirtualDiskFlatVer2BackingInfo)
                    {
                        VirtualDiskFlatVer2BackingInfo backing = (VirtualDiskFlatVer2BackingInfo)virtualDevice.backing;
                        vmdkProperties.DiskMode = backing.diskMode;
                        if (backing.thinProvisionedSpecified)
                            vmdkProperties.ThinProvisioned = backing.thinProvisioned;
                        if (!string.IsNullOrEmpty(backing.uuid))
                            vmdkProperties.Guid = new Guid(backing.uuid);
                    }
                    else if (virtualDevice.backing is VirtualDiskRawDiskMappingVer1BackingInfo)
                    {
                        VirtualDiskRawDiskMappingVer1BackingInfo backing = (VirtualDiskRawDiskMappingVer1BackingInfo)virtualDevice.backing;
                        vmdkProperties.DiskMode = backing.diskMode;
                        if (!string.IsNullOrEmpty(backing.uuid))
                            vmdkProperties.Guid = new Guid(backing.uuid);
                    }
                    else if (virtualDevice.backing is VirtualDiskSparseVer1BackingInfo)
                        vmdkProperties.DiskMode = ((VirtualDiskSparseVer1BackingInfo)virtualDevice.backing).diskMode;
                    else if (virtualDevice.backing is VirtualDiskSparseVer2BackingInfo)
                    {
                        VirtualDiskSparseVer2BackingInfo backing = (VirtualDiskSparseVer2BackingInfo)virtualDevice.backing;
                        vmdkProperties.DiskMode = backing.diskMode;
                        if (!string.IsNullOrEmpty(backing.uuid))
                            vmdkProperties.Guid = new Guid(backing.uuid);
                    }
                    vmdkProperties.Capacity = ((VirtualDisk)virtualDevice).capacityInKB;
                    vmdkProperties.Key = virtualDevice.key;
                    vmdkProperties.UnitNumber = virtualDevice.unitNumber;
                    vmdkProperties.ControllerKey = virtualDevice.controllerKey;
                    vmdkProperties.BusNumber = -1;
                    vmdkProperties.Label = virtualDevice.deviceInfo.label;
                    IVimDatastore vmdkDatastore = this.GetVmdkDatastore(vmdkProperties.FileName, datastores);
                    vmdkProperties.Datastore = vmdkDatastore;
                    vmdkProperties.DatastoreName = vmdkDatastore.DsProperties.Name;
                    vmdkProperties.DatastoreUrl = vmdkDatastore.DsProperties.Url;
                    vmdkPropertiesList.Add(vmdkProperties);
                    if (!(virtualDevice is VirtualIDEController))
                        ;
                }
                else if (virtualDevice is VirtualSCSIController)
                    virtualScsiControllerList.Add((VirtualSCSIController)virtualDevice);
                else if (virtualDevice is VirtualIDEController)
                    virtualIdeControllerList.Add((VirtualIDEController)virtualDevice);
            }
            for (int index2 = 0; index2 < vmdkPropertiesList.Count; ++index2)
            {
                VmdkProperties vmdkProperties = vmdkPropertiesList[index2];
                foreach (VirtualSCSIController virtualScsiController in virtualScsiControllerList)
                {
                    if (vmdkProperties.ControllerKey == virtualScsiController.key)
                    {
                        vmdkProperties.BusNumber = virtualScsiController.busNumber;
                        break;
                    }
                }
            }
            for (int index2 = 0; index2 < vmdkPropertiesList.Count; ++index2)
            {
                VmdkProperties vmdkProperties = vmdkPropertiesList[index2];
                foreach (VirtualIDEController virtualIdeController in virtualIdeControllerList)
                {
                    if (vmdkProperties.ControllerKey == virtualIdeController.key)
                    {
                        vmdkProperties.BusNumber = virtualIdeController.busNumber + 1000;
                        vmdkProperties.UnitNumber += 1000;
                        break;
                    }
                }
            }
            return vmdkPropertiesList.ToArray();
        }

        public string GetVmxFullName()
        {
            IVimDatastore primaryDatastore = this.GetPrimaryDatastore();
            string rawVmxPath = this.GetRawVmxPath();
            string path = primaryDatastore.GetPath();
            return this.GetVmxFullName(primaryDatastore, rawVmxPath, path);
        }

        public string GetVmxFullName(IVimDatastore datastore, string rawVmxPath, string datastorePath)
        {
            if (datastore == null)
                return (string)null;
            string[] strArray = rawVmxPath.Split(new string[1] { "] " }, StringSplitOptions.None);
            if (strArray == null)
                return rawVmxPath;
            if (strArray[1].StartsWith("/"))
                return datastorePath + strArray[1];
            return datastorePath + "/" + strArray[1];
        }

        public string GetVmxPath(IVimDatastore datastore, string rawVmxPath, string datastorePath)
        {
            return CUtils.GetDirectoryName(this.GetVmxFullName(datastore, rawVmxPath, datastorePath)).Replace('\\', '/');
        }

        public override string GetName()
        {
            if (string.IsNullOrEmpty(this.Name))
                this.GetCommonProperties();
            return this.Name;
        }

        private IVimDatastore GetVmdkDatastore(string vmdkFileName, IVimDatastore[] datastores)
        {
            IVimDatastore vimDatastore = (IVimDatastore)null;
            if (datastores != null)
            {
                foreach (IVimDatastore datastore in datastores)
                {
                    if (vmdkFileName.Contains("[" + datastore.Name + "]"))
                    {
                        vimDatastore = datastore;
                        break;
                    }
                }
            }
            return vimDatastore;
        }

        public IVimNetwork[] GetNetworksAndProperties()
        {
            return this.GetNetworksAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "network", "Network", Network.VCProperties));
        }

        public IVimNetwork[] GetNetworksAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties)
        {
            List<Network> networkList = new List<Network>();
            foreach (ManagedObjectAndProperties objectsAndProperty in managedObjectsAndProperties)
            {
                if ((!(objectsAndProperty.ManagedObject.type != "Network") || !(objectsAndProperty.ManagedObject.type != "DistributedVirtualPortgroup")) && objectsAndProperty.Properties != null)
                {
                    Network network = new Network(this.VcService, objectsAndProperty.ManagedObject);
                    network.GetCommonProperties(objectsAndProperty.Properties);
                    networkList.Add(network);
                }
            }
            return (IVimNetwork[])networkList.ToArray();
        }

        public void SetNicConnectivity(bool enableConnectivity, VimClientlContext ctx)
        {
            Dictionary<string, VirtualEthernetCard> ethernetCards = this.GetEthernetCards();
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            foreach (VirtualEthernetCard virtualEthernetCard in ethernetCards.Values)
            {
                virtualEthernetCard.connectable.startConnected = enableConnectivity;
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    device = (VirtualDevice)virtualEthernetCard,
                    operation = VirtualDeviceConfigSpecOperation.edit,
                    operationSpecified = true
                });
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("SetNicConnectivity", ctx);
        }

        public void SetNicVirtualSwitches(string[] virtualSwitchNames, VimClientlContext ctx)
        {
            if (virtualSwitchNames == null || virtualSwitchNames.Length == 0)
                return;
            virtualSwitchNames = this._filterDiscardedNics(virtualSwitchNames);
            IVimHost withoutProperties = this.GetHostWithoutProperties();
            IVimNetwork[] networks = withoutProperties.GetNetworks();
            Dictionary<string, string> virtualSwitchUuids = withoutProperties.GetDistributedVirtualSwitchUuids();
            Dictionary<string, IVimNetwork> networksDict = new Dictionary<string, IVimNetwork>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            ((IEnumerable<IVimNetwork>)networks).ForEach<IVimNetwork>((System.Action<IVimNetwork>)(t => networksDict.Add(t.Name, t)));
            Dictionary<string, VirtualEthernetCard> ethernetCards = this.GetEthernetCards();
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            int index = 0;
            foreach (VirtualEthernetCard virtualEthernetCard in ethernetCards.Values)
            {
                if (virtualSwitchNames.Length > index && networksDict.ContainsKey(virtualSwitchNames[index]))
                {
                    virtualEthernetCard.connectable.connected = true;
                    virtualEthernetCard.connectable.startConnected = true;
                    IVimNetwork vimNetwork = networksDict[virtualSwitchNames[index]];
                    if (!vimNetwork.IsDistributed)
                    {
                        virtualEthernetCard.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardNetworkBackingInfo();
                        ((VirtualEthernetCardNetworkBackingInfo)virtualEthernetCard.backing).network = vimNetwork.ManagedObject;
                        ((VirtualDeviceDeviceBackingInfo)virtualEthernetCard.backing).deviceName = vimNetwork.Name;
                    }
                    else if (virtualSwitchUuids.ContainsKey(vimNetwork.PortgroupKey))
                    {
                        virtualEthernetCard.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardDistributedVirtualPortBackingInfo();
                        ((VirtualEthernetCardDistributedVirtualPortBackingInfo)virtualEthernetCard.backing).port = new DistributedVirtualSwitchPortConnection()
                        {
                            switchUuid = virtualSwitchUuids[vimNetwork.PortgroupKey],
                            portgroupKey = vimNetwork.PortgroupKey
                        };
                    }
                }
                else
                {
                    virtualEthernetCard.connectable.connected = false;
                    virtualEthernetCard.connectable.startConnected = false;
                    IVimNetwork vimNetwork = networksDict.First<KeyValuePair<string, IVimNetwork>>().Value;
                    if (!vimNetwork.IsDistributed)
                    {
                        virtualEthernetCard.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardNetworkBackingInfo();
                        ((VirtualEthernetCardNetworkBackingInfo)virtualEthernetCard.backing).network = vimNetwork.ManagedObject;
                        ((VirtualDeviceDeviceBackingInfo)virtualEthernetCard.backing).deviceName = vimNetwork.Name;
                    }
                    else if (virtualSwitchUuids.ContainsKey(vimNetwork.PortgroupKey))
                    {
                        virtualEthernetCard.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardDistributedVirtualPortBackingInfo();
                        ((VirtualEthernetCardDistributedVirtualPortBackingInfo)virtualEthernetCard.backing).port = new DistributedVirtualSwitchPortConnection()
                        {
                            switchUuid = virtualSwitchUuids[vimNetwork.PortgroupKey],
                            portgroupKey = vimNetwork.PortgroupKey
                        };
                    }
                }
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    device = (VirtualDevice)virtualEthernetCard,
                    operation = VirtualDeviceConfigSpecOperation.edit,
                    operationSpecified = true
                });
                ++index;
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("SetNicVirtualSwitches", ctx);
        }

        private string[] _filterDiscardedNics(string[] nicsInfo)
        {
            List<string> stringList = new List<string>();
            foreach (string str in nicsInfo)
            {
                if (!str.Equals("---Discard---"))
                    stringList.Add(str);
            }
            return stringList.ToArray();
        }

        public IVimSnapshot GetSnapshot(string snapName)
        {
            if (snapName == null || snapName == "")
                return (IVimSnapshot)null;
            VirtualMachineSnapshotInfo snapshotInfo = this.GetSnapshotInfo();
            if (snapshotInfo == null)
                return (IVimSnapshot)null;
            return this.TraverseSnapshotInTree(snapshotInfo.rootSnapshotList, snapName);
        }

        private IVimSnapshot TraverseSnapshotInTree(VirtualMachineSnapshotTree[] snapTree, string snapName)
        {
            if (snapTree == null)
                return (IVimSnapshot)null;
            IVimSnapshot vimSnapshot = (IVimSnapshot)null;
            for (int index = 0; index < snapTree.Length && vimSnapshot == null; ++index)
            {
                VirtualMachineSnapshotTree machineSnapshotTree = snapTree[index];
                vimSnapshot = !(machineSnapshotTree.name == snapName) ? this.TraverseSnapshotInTree(machineSnapshotTree.childSnapshotList, snapName) : (IVimSnapshot)new Snapshot(this.VcService, machineSnapshotTree.snapshot);
            }
            return vimSnapshot;
        }

        private List<string> GetSnapshotNames()
        {
            List<string> snapshotList = new List<string>();
            VirtualMachineSnapshotInfo snapshotInfo = this.GetSnapshotInfo();
            if (snapshotInfo != null)
                this.TraverseSnapshotInTree(snapshotInfo.rootSnapshotList, ref snapshotList);
            return snapshotList;
        }

        private bool HasSnapshots()
        {
            VirtualMachineSnapshotInfo snapshotInfo = this.GetSnapshotInfo();
            if (snapshotInfo == null)
                return false;
            VirtualMachineSnapshotTree[] rootSnapshotList = snapshotInfo.rootSnapshotList;
            if (rootSnapshotList != null)
                return (uint)rootSnapshotList.Length > 0U;
            return false;
        }

        private void TraverseSnapshotInTree(VirtualMachineSnapshotTree[] snapTree, ref List<string> snapshotList)
        {
            if (snapTree == null)
                return;
            for (int index = 0; index < snapTree.Length; ++index)
            {
                VirtualMachineSnapshotTree machineSnapshotTree = snapTree[index];
                snapshotList.Add(machineSnapshotTree.name);
                this.TraverseSnapshotInTree(machineSnapshotTree.childSnapshotList, ref snapshotList);
            }
        }

        private VirtualMachineSnapshotInfo GetSnapshotInfo()
        {
            Dictionary<string, object> properties = this.GetProperties(new string[1] { "snapshot" });
            object obj = (object)null;
            string key = "snapshot";
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            object local = @obj;
            if (!properties.TryGetValue(key, out local))
                return (VirtualMachineSnapshotInfo)null;
            return (VirtualMachineSnapshotInfo)obj;
        }

        public void RemoveSnapshot(string snapName, VimClientlContext ctx)
        {
            if (this._logger != null)
                this._logger.Verbose("RemoveSnapshot: begin", "Vm");
            IVimSnapshot snapshot = this.GetSnapshot(snapName);
            if (snapshot == null)
            {
                if (this._logger != null)
                    this._logger.Verbose("RemoveSnapshot: error: no such snapshot: " + snapName, "Vm");
                throw new EsxException("No such snapshot: " + snapName, false);
            }
            ManagedObjectReference managedObject = (ManagedObjectReference)null;
            try
            {
                managedObject = this.VcService.Service.RemoveSnapshot_Task(snapshot.ManagedObject, false, true, true);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout)
                    throw;
                else if (this._logger != null)
                    this._logger.Information("RemoveSnapshot: Create_Task timeout (OK): proceed with WaitForResult", "Vm");
            }
            ctx.IsRetriableCall = false;
            if (managedObject != null)
                new Task(this.VcService, managedObject).WaitForResult("RemoveSnapshot", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("RemoveSnapshot: end", "Vm");
        }

        public void RemoveSnapshot(ManagedObjectReference mor, VimClientlContext ctx)
        {
            if (this._logger != null)
                this._logger.Verbose("RemoveSnapshot: begin", "Vm");
            ManagedObjectReference managedObject = (ManagedObjectReference)null;
            try
            {
                managedObject = this.VcService.Service.RemoveSnapshot_Task(mor, false, true, true);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout)
                    throw;
                else if (this._logger != null)
                    this._logger.Information("RemoveSnapshot: Create_Task timeout (OK): proceed with WaitForResult", "Vm");
            }
            ctx.IsRetriableCall = false;
            if (managedObject != null)
                new Task(this.VcService, managedObject).WaitForResult("RemoveSnapshot", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("RemoveSnapshot: end", "Vm");
        }

        public void RemoveAllSnapshots(VimClientlContext ctx)
        {
            if (this._logger != null)
                this._logger.Verbose("RemoveAllSnapshots: begin", "Vm");
            if (!this.HasSnapshots())
            {
                if (this._logger == null)
                    return;
                this._logger.Verbose("RemoveAllSnapshots: no snapshots", "Vm");
            }
            else
            {
                ManagedObjectReference managedObject = (ManagedObjectReference)null;
                try
                {
                    managedObject = this.VcService.Service.RemoveAllSnapshots_Task(this.ManagedObject, true, true);
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.Timeout)
                        throw;
                    else if (this._logger != null)
                        this._logger.Information("RemoveAllSnapshots: Create_Task timeout (OK): proceed with WaitForResult", "Vm");
                }
                ctx.IsRetriableCall = false;
                if (managedObject != null)
                    new Task(this.VcService, managedObject).WaitForResult("RemoveAllSnapshots", ctx);
                if (this._logger == null)
                    return;
                this._logger.Verbose("RemoveAllSnapshots: end", "Vm");
            }
        }

        public void TryRemoveAllSnapshots()
        {
            this.VcService.Service.RemoveAllSnapshots_Task(this.ManagedObject, true, true);
        }

        public void CreateSnapshot(string snapName, string description, VimClientlContext ctx)
        {
            this.CreateSnapshot(snapName, description, ctx, false, false);
        }

        public void CreateSnapshot(string snapName, string description, VimClientlContext ctx, bool bIncludeMemory, bool bQuiesceFileSystem)
        {
            if (this._logger != null)
                this._logger.Verbose("CreateSnapshot: begin", "Vm");
            IVimSnapshot snapshot = this.GetSnapshot(snapName);
            if (snapshot != null && this._logger != null)
            {
                this._logger.Verbose("CreateSnapshot: error: snapshot exists: " + snapshot.Name, "Vm");
                throw new EsxException("CreateSnapshot: snapshot already exists", false);
            }
            ManagedObjectReference managedObject = (ManagedObjectReference)null;
            try
            {
                managedObject = this.VcService.Service.CreateSnapshot_Task(this.ManagedObject, snapName, description, bIncludeMemory, bQuiesceFileSystem);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.Timeout)
                    throw;
                else if (this._logger != null)
                    this._logger.Information("CreateSnapshot: Create_Task timeout (OK): proceed with WaitForResult", "Vm");
            }
            ctx.IsRetriableCall = false;
            if (managedObject != null)
                new Task(this.VcService, managedObject).WaitForResult("CreateSnapshot", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("CreateSnapshot: end", "Vm");
        }

        public bool IsSuspended()
        {
            string index = "runtime.powerState";
            return (VirtualMachinePowerState)this.GetProperties(new string[1] { index })[index] == VirtualMachinePowerState.suspended;
        }

        public void AssertSnapshots(string[] expectedSnapshotNames)
        {
            int num = 0;
            List<string> snapshotNames = this.GetSnapshotNames();
            int count = snapshotNames.Count;
            if (expectedSnapshotNames != null)
                num = expectedSnapshotNames.Length;
            if (count != num)
                throw new EsxException("The number of snapshots expected: " + (object)num + ", actual: " + (object)count, true);
            if (expectedSnapshotNames == null)
                return;
            foreach (string expectedSnapshotName in expectedSnapshotNames)
            {
                if (!snapshotNames.Contains(expectedSnapshotName))
                    throw new EsxException("No such snapshot: " + expectedSnapshotName, true);
            }
        }

        public void RevertToLastSnapshot(VimClientlContext ctx)
        {
            if (this._logger != null)
                this._logger.Verbose("RevertToLastSnapshot: begin", "Vm");
            if (this.GetSnapshotNames().Count == 0)
            {
                if (this._logger != null)
                    this._logger.Verbose("RevertToLastSnapshot: error: snapshot does not exist", "Vm");
                throw new EsxException("RevertToLastSnapshot: snapshot does not exist", false);
            }
            ManagedObjectReference currentSnapshotTask = this.VcService.Service.RevertToCurrentSnapshot_Task(this.ManagedObject, (ManagedObjectReference)null, false, false);
            ctx.IsRetriableCall = false;
            new Task(this.VcService, currentSnapshotTask).WaitForResult("RevertToLastSnapshot", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("RevertToLastSnapshot: end", "Vm");
        }

        public void RevertToSnapshot(string snapName, VimClientlContext ctx)
        {
            if (this._logger != null)
                this._logger.Verbose("RevertToSnapshot: begin", "Vm");
            IVimSnapshot snapshot = this.GetSnapshot(snapName);
            if (snapshot == null)
            {
                if (this._logger != null)
                    this._logger.Verbose("RevertToSnapshot: error: snapshot does not exist: " + snapName, "Vm");
                throw new EsxException("RevertToSnapshot: snapshot does not exist", false);
            }
            ManagedObjectReference snapshotTask = this.VcService.Service.RevertToSnapshot_Task(snapshot.ManagedObject, (ManagedObjectReference)null, false, false);
            ctx.IsRetriableCall = false;
            new Task(this.VcService, snapshotTask).WaitForResult("RevertToSnapshot", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("RevertToSnapshot: end", "Vm");
        }

        public void RemoveVirtualDisks(string[] names, VimClientlContext ctx)
        {
            if (names == null || names.Length == 0)
                return;
            List<string> virtualDisksFileNames;
            this.removeVirtualDisks(this.GetVirtualDisks(names, out virtualDisksFileNames), ctx);
            this.VerifyVirtualDisksRemoved(virtualDisksFileNames);
        }

        public void RemoveVirtualDisksByFilename(string[] filenames, VimClientlContext ctx)
        {
            if (filenames == null || filenames.Length == 0)
                return;
            this.removeVirtualDisks(this.GetVirtualDisksByFilename(filenames), ctx);
            this.VerifyVirtualDisksRemoved(filenames.ToList<string>());
        }

        public void RemoveVirtualDisks(VmdkProperties[] VmdkProps, VimClientlContext ctx)
        {
            if (VmdkProps == null || VmdkProps.Length == 0)
                return;
            List<string> virtualDisksFileNames;
            this.removeVirtualDisks(this.GetVirtualDisksByGuid(VmdkProps, out virtualDisksFileNames), ctx);
            this.VerifyVirtualDisksRemoved(virtualDisksFileNames);
        }

        private void removeVirtualDisks(VirtualDisk[] virtualDisks, VimClientlContext ctx)
        {
            if (virtualDisks.Length == 0)
                return;
            VirtualDeviceConfigSpec[] deviceConfigSpecArray = new VirtualDeviceConfigSpec[virtualDisks.Length];
            for (int index = 0; index < virtualDisks.Length; ++index)
            {
                VirtualDisk virtualDisk = virtualDisks[index];
                deviceConfigSpecArray[index] = new VirtualDeviceConfigSpec();
                deviceConfigSpecArray[index].device = (VirtualDevice)virtualDisk;
                deviceConfigSpecArray[index].operation = VirtualDeviceConfigSpecOperation.remove;
                deviceConfigSpecArray[index].operationSpecified = true;
            }
            VirtualMachineConfigSpec spec = new VirtualMachineConfigSpec();
            spec.deviceChange = deviceConfigSpecArray;
            ctx.IsRetriableCall = false;
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, spec)).WaitForResult("ReconfigVM", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("RemoveVirtualDisks: end", "Vm");
        }

        private void deleteVirtualDisks(VirtualDisk[] virtualDisks, VimClientlContext ctx)
        {
            if (virtualDisks.Length == 0)
                return;
            VirtualDeviceConfigSpec[] deviceConfigSpecArray = new VirtualDeviceConfigSpec[virtualDisks.Length];
            for (int index = 0; index < virtualDisks.Length; ++index)
            {
                VirtualDisk virtualDisk = virtualDisks[index];
                deviceConfigSpecArray[index] = new VirtualDeviceConfigSpec();
                deviceConfigSpecArray[index].device = (VirtualDevice)virtualDisk;
                deviceConfigSpecArray[index].operation = VirtualDeviceConfigSpecOperation.remove;
                deviceConfigSpecArray[index].operationSpecified = true;
                deviceConfigSpecArray[index].fileOperation = VirtualDeviceConfigSpecFileOperation.destroy;
                deviceConfigSpecArray[index].fileOperationSpecified = true;
            }
            VirtualMachineConfigSpec spec = new VirtualMachineConfigSpec();
            spec.deviceChange = deviceConfigSpecArray;
            ctx.IsRetriableCall = false;
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, spec)).WaitForResult("ReconfigVM", ctx);
            if (this._logger == null)
                return;
            this._logger.Verbose("RemoveVirtualDisks: end", "Vm");
        }

        private VirtualDisk[] GetVirtualDisksByGuid(VmdkProperties[] VmdkProps, out List<string> virtualDisksFileNames)
        {
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            virtualDisksFileNames = new List<string>();
            List<VirtualDisk> virtualDiskList = new List<VirtualDisk>();
            List<Guid> list = ((IEnumerable<VmdkProperties>)VmdkProps).Select<VmdkProperties, Guid>((Func<VmdkProperties, Guid>)(x => x.Guid)).ToList<Guid>();
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualDisk)
                {
                    Guid guidToCompare = Vm.getGuidToCompare(virtualDevice);
                    if (list.Remove(guidToCompare))
                    {
                        virtualDisksFileNames.Add(((VirtualDeviceFileBackingInfo)virtualDevice.backing).fileName);
                        virtualDiskList.Add((VirtualDisk)virtualDevice);
                    }
                }
            }
            if (list.Any<Guid>())
                this._logger.Verbose(string.Format("Could not find GUID to delete: {0}", (object)string.Join(", ", list.Select<Guid, string>((Func<Guid, string>)(s => s.ToString())))), "GetVirtualDisks");
            return virtualDiskList.ToArray();
        }

        private static Guid getGuidToCompare(VirtualDevice virtualDevice)
        {
            if (virtualDevice.backing is VirtualDiskFlatVer2BackingInfo)
                return new Guid(((VirtualDiskFlatVer2BackingInfo)virtualDevice.backing).uuid);
            if (virtualDevice.backing is VirtualDiskPartitionedRawDiskVer2BackingInfo)
                return new Guid(((VirtualDiskRawDiskVer2BackingInfo)virtualDevice.backing).uuid);
            if (virtualDevice.backing is VirtualDiskSparseVer2BackingInfo)
                return new Guid(((VirtualDiskSparseVer2BackingInfo)virtualDevice.backing).uuid);
            return Guid.Empty;
        }

        private VirtualDisk[] GetVirtualDisks(string[] names, out List<string> virtualDisksFileNames)
        {
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            virtualDisksFileNames = new List<string>();
            List<VirtualDisk> virtualDiskList = new List<VirtualDisk>();
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualDisk)
                {
                    foreach (string name in names)
                    {
                        if (string.Compare(virtualDevice.deviceInfo.label, name, true, CultureInfo.InvariantCulture) == 0)
                        {
                            virtualDisksFileNames.Add(((VirtualDeviceFileBackingInfo)virtualDevice.backing).fileName);
                            VirtualDisk virtualDisk = (VirtualDisk)virtualDevice;
                            virtualDiskList.Add(virtualDisk);
                        }
                    }
                }
            }
            return virtualDiskList.ToArray();
        }

        private VirtualDisk[] GetVirtualDisksByFilename(string[] names)
        {
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            List<VirtualDisk> virtualDiskList = new List<VirtualDisk>();
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualDisk)
                {
                    string fileName = ((VirtualDeviceFileBackingInfo)virtualDevice.backing).fileName;
                    if (((IEnumerable<string>)names).Contains<string>(fileName, (IEqualityComparer<string>)new Vm.VMWareFilenameEqualityComparer()))
                        virtualDiskList.Add((VirtualDisk)virtualDevice);
                }
            }
            return virtualDiskList.ToArray();
        }

        private void VerifyVirtualDisksRemoved(List<string> virtualDisksFileNames)
        {
            string str = (string)null;
            for (int index = 0; index < 36; ++index)
            {
                str = (string)null;
                Dictionary<string, string> virtualDiskLabels = this.GetVirtualDiskLabels();
                foreach (string virtualDisksFileName in virtualDisksFileNames)
                {
                    if (virtualDiskLabels.ContainsKey(virtualDisksFileName))
                    {
                        str = virtualDisksFileName;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(str))
                    Thread.Sleep(5000);
                else
                    break;
            }
            if (!string.IsNullOrEmpty(str))
                throw new ApplicationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Virtual disk {0} has not been removed from the virtual machine.", new object[1] { (object)str }));
        }

        private void VerifyVirtualDisksRemoved()
        {
            bool flag = false;
            for (int index = 0; index < 36; ++index)
            {
                if (this.GetVirtualDiskLabels().Count == 0)
                {
                    flag = true;
                    break;
                }
                Thread.Sleep(5000);
            }
            if (!flag)
                throw new ApplicationException("All the virtual disk have not been removed from the virtual machine.");
        }

        public void RemoveAllVirtualDisks(RemoveDisk removeDisk, VimClientlContext ctx)
        {
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualDisk)
                {
                    VirtualDeviceConfigSpec deviceConfigSpec = new VirtualDeviceConfigSpec();
                    deviceConfigSpec.device = virtualDevice;
                    deviceConfigSpec.operation = VirtualDeviceConfigSpecOperation.remove;
                    deviceConfigSpec.operationSpecified = true;
                    if (removeDisk == RemoveDisk.DetachAndDelete)
                    {
                        deviceConfigSpec.fileOperation = VirtualDeviceConfigSpecFileOperation.destroy;
                        deviceConfigSpec.fileOperationSpecified = true;
                    }
                    deviceConfigSpecList.Add(deviceConfigSpec);
                }
            }
            VirtualMachineConfigSpec spec = new VirtualMachineConfigSpec();
            spec.deviceChange = deviceConfigSpecList.ToArray();
            ctx.IsRetriableCall = false;
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, spec)).WaitForResult("ReconfigVM", ctx);
            this.VerifyVirtualDisksRemoved();
            if (this._logger == null)
                return;
            this._logger.Verbose("RemoveAllVirtualDisks: end", "Vm");
        }

        public bool HasUserDefinedSnapshots()
        {
            foreach (string snapshotName in this.GetSnapshotNames())
            {
                if (!snapshotName.Contains(this.Uuid) && snapshotName != "Consolidate Helper")
                    return true;
            }
            return false;
        }

        private static bool IsSnapshottableDisk(string diskMode)
        {
            return !diskMode.Contains("independent");
        }

        public VmConfiguration GetConfiguration()
        {
            VmConfiguration vmConfiguration = new VmConfiguration();
            List<VSwitchInfo> vswitchInfoList = new List<VSwitchInfo>();
            Dictionary<string, object> properties = this.GetProperties(new string[4] { "config.version", "config.hardware.numCPU", "config.hardware.memoryMB", "config.hardware.device" });
            vmConfiguration.NumCPU = (int)Convert.ToInt16(properties["config.hardware.numCPU"]);
            string str = (string)properties["config.version"];
            if (str.Equals("vmx-07"))
            {
                vmConfiguration.VmVersion = 7;
            }
            else
            {
                if (!str.Equals("vmx-04"))
                    throw new EsxException("Unsupported VM version: " + str, false);
                vmConfiguration.VmVersion = 4;
            }
            vmConfiguration.MemoryMB = Convert.ToInt64(properties["config.hardware.memoryMB"]);
            VirtualDevice[] virtualDeviceArray = (VirtualDevice[])properties["config.hardware.device"];
            IVimHost hostAndProperties = this.GetHostAndProperties();
            IVimNetwork[] networksAndProperties = this.GetNetworksAndProperties();
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            foreach (Network network in networksAndProperties)
            {
                if (network.IsDistributed)
                    CUtils.AddOrReplace<string, string>((IDictionary<string, string>)dictionary, network.PortgroupKey, network.Name);
            }
            foreach (VirtualDevice virtualDevice in virtualDeviceArray)
            {
                if (virtualDevice is VirtualEthernetCard)
                {
                    VSwitchInfo vswitchInfo = new VSwitchInfo();
                    if (hostAndProperties.Properties.Version.StartsWith("3.") && !CUtils.ContainsDigit(virtualDevice.deviceInfo.label))
                    {
                        vswitchInfo.Name = virtualDevice.deviceInfo.label + "-" + (object)virtualDevice.key;
                        vswitchInfo.VirtualNetwork = ((VirtualDeviceDeviceBackingInfo)virtualDevice.backing).deviceName;
                    }
                    else
                    {
                        vswitchInfo.Name = virtualDevice.deviceInfo.label;
                        if (virtualDevice.backing is VirtualEthernetCardDistributedVirtualPortBackingInfo)
                        {
                            string portgroupKey = ((VirtualEthernetCardDistributedVirtualPortBackingInfo)virtualDevice.backing).port.portgroupKey;
                            if (dictionary.ContainsKey(portgroupKey))
                                vswitchInfo.VirtualNetwork = dictionary[portgroupKey];
                        }
                        else
                            vswitchInfo.VirtualNetwork = virtualDevice.deviceInfo.summary;
                    }
                    vswitchInfo.ConnectAtPowerOn = virtualDevice.connectable.startConnected;
                    vswitchInfoList.Add(vswitchInfo);
                }
            }
            vmConfiguration.NetworkAdapters = vswitchInfoList.ToArray();
            return vmConfiguration;
        }

        private void AddNetwork(ManagedObjectReference mor, VimClientlContext ctx)
        {
            VirtualPCNet32 virtualPcNet32 = new VirtualPCNet32();
            VirtualEthernetCardNetworkBackingInfo networkBackingInfo = new VirtualEthernetCardNetworkBackingInfo();
            networkBackingInfo.deviceName = "VM Network 2";
            networkBackingInfo.network = mor;
            virtualPcNet32.backing = (VirtualDeviceBackingInfo)networkBackingInfo;
            virtualPcNet32.deviceInfo = new Description();
            virtualPcNet32.deviceInfo.label = "";
            virtualPcNet32.deviceInfo.summary = "";
            virtualPcNet32.key = 1;
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = new List<VirtualDeviceConfigSpec>()
        {
          new VirtualDeviceConfigSpec()
          {
            device = (VirtualDevice) virtualPcNet32,
            operation = VirtualDeviceConfigSpecOperation.add,
            operationSpecified = true
          }
        }.ToArray()
            })).WaitForResult("Add network", ctx);
        }

        public void EditNetwork(VirtualEthernetCard[] ethernetCards, Network network, VimClientlContext ctx)
        {
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            foreach (VirtualEthernetCard ethernetCard in ethernetCards)
            {
                ((VirtualEthernetCardNetworkBackingInfo)ethernetCard.backing).network = network.ManagedObject;
                ((VirtualDeviceDeviceBackingInfo)ethernetCard.backing).deviceName = network.Name;
                ethernetCard.deviceInfo.label = "";
                ethernetCard.deviceInfo.summary = "";
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    device = (VirtualDevice)ethernetCard,
                    operation = VirtualDeviceConfigSpecOperation.edit,
                    operationSpecified = true
                });
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("Edit network", ctx);
        }

        public void SetConfiguration(FailoverConfig failoverConfig, VimClientlContext ctx)
        {
            this.SetConfiguration(failoverConfig, true, ctx);
        }

        public void SetConfiguration(FailoverConfig failoverConfig, bool disableAllNics, VimClientlContext ctx)
        {
            VirtualMachineConfigSpec spec = new VirtualMachineConfigSpec();
            spec.numCPUs = failoverConfig.TargetNumCPU;
            spec.numCPUsSpecified = true;
            spec.memoryMB = failoverConfig.TargetRamMB;
            spec.memoryMBSpecified = true;
            IVimHost withoutProperties = this.GetHostWithoutProperties();
            IVimNetwork[] networks = withoutProperties.GetNetworks();
            Dictionary<string, string> virtualSwitchUuids = withoutProperties.GetDistributedVirtualSwitchUuids();
            Dictionary<string, Network> dictionary = new Dictionary<string, Network>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            foreach (FailoverConfigNic nicMapping in failoverConfig.NicMappings)
            {
                foreach (Network network in networks)
                {
                    if (string.Compare(nicMapping.TargetNetwork, network.Name, true, CultureInfo.CurrentCulture) == 0)
                    {
                        if (!dictionary.ContainsKey(network.Name))
                        {
                            dictionary.Add(network.Name, network);
                            break;
                        }
                        break;
                    }
                }
            }
            Dictionary<string, VirtualEthernetCard> ethernetCards = this.GetEthernetCards();
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            foreach (FailoverConfigNic nicMapping in failoverConfig.NicMappings)
            {
                if (ethernetCards.ContainsKey(nicMapping.Name))
                {
                    VirtualEthernetCard virtualEthernetCard = ethernetCards[nicMapping.Name];
                    if (dictionary.ContainsKey(nicMapping.TargetNetwork))
                    {
                        Network network = dictionary[nicMapping.TargetNetwork];
                        string empty = string.Empty;
                        if (virtualSwitchUuids.ContainsKey(network.PortgroupKey))
                            empty = virtualSwitchUuids[network.PortgroupKey];
                        if (virtualEthernetCard.backing is VirtualEthernetCardNetworkBackingInfo && !network.IsDistributed)
                        {
                            ((VirtualEthernetCardNetworkBackingInfo)virtualEthernetCard.backing).network = network.ManagedObject;
                            ((VirtualDeviceDeviceBackingInfo)virtualEthernetCard.backing).deviceName = network.Name;
                        }
                        else if (virtualEthernetCard.backing is VirtualEthernetCardDistributedVirtualPortBackingInfo && network.IsDistributed)
                            ((VirtualEthernetCardDistributedVirtualPortBackingInfo)virtualEthernetCard.backing).port = new DistributedVirtualSwitchPortConnection()
                            {
                                switchUuid = empty,
                                portgroupKey = network.PortgroupKey
                            };
                        else if (virtualEthernetCard.backing is VirtualEthernetCardNetworkBackingInfo && network.IsDistributed)
                        {
                            virtualEthernetCard.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardDistributedVirtualPortBackingInfo();
                            ((VirtualEthernetCardDistributedVirtualPortBackingInfo)virtualEthernetCard.backing).port = new DistributedVirtualSwitchPortConnection()
                            {
                                switchUuid = empty,
                                portgroupKey = network.PortgroupKey
                            };
                        }
                        if (virtualEthernetCard.backing is VirtualEthernetCardDistributedVirtualPortBackingInfo && !network.IsDistributed)
                        {
                            virtualEthernetCard.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardNetworkBackingInfo();
                            ((VirtualEthernetCardNetworkBackingInfo)virtualEthernetCard.backing).network = network.ManagedObject;
                            ((VirtualDeviceDeviceBackingInfo)virtualEthernetCard.backing).deviceName = network.Name;
                        }
                        if (disableAllNics)
                            virtualEthernetCard.connectable.startConnected = false;
                        virtualEthernetCard.deviceInfo.label = "";
                        virtualEthernetCard.deviceInfo.summary = "";
                        deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                        {
                            device = (VirtualDevice)virtualEthernetCard,
                            operation = VirtualDeviceConfigSpecOperation.edit,
                            operationSpecified = true
                        });
                    }
                }
            }
            spec.deviceChange = deviceConfigSpecList.ToArray();
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, spec)).WaitForResult("Set Configuration", ctx);
        }

        public VirtualEthernetCard[] GetEthernetCards(IVimNetwork network)
        {
            List<VirtualEthernetCard> virtualEthernetCardList = new List<VirtualEthernetCard>();
            string[] properties = new string[1] { "config.hardware.device" };
            foreach (VirtualDevice virtualDevice in (VirtualDevice[])this.GetProperties(properties)["config.hardware.device"])
            {
                if (virtualDevice is VirtualEthernetCard && network.Name == ((VirtualDeviceDeviceBackingInfo)virtualDevice.backing).deviceName)
                    virtualEthernetCardList.Add((VirtualEthernetCard)virtualDevice);
            }
            return virtualEthernetCardList.ToArray();
        }

        public Dictionary<string, VirtualEthernetCard> GetEthernetCards()
        {
            Dictionary<string, VirtualEthernetCard> dictionary = new Dictionary<string, VirtualEthernetCard>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            IVimHost hostAndProperties = this.GetHostAndProperties();
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualEthernetCard)
                {
                    if (hostAndProperties.Properties.Version.StartsWith("3.") && !CUtils.ContainsDigit(virtualDevice.deviceInfo.label))
                        dictionary.Add(virtualDevice.deviceInfo.label + "-" + (object)virtualDevice.key, (VirtualEthernetCard)virtualDevice);
                    else
                        dictionary.Add(virtualDevice.deviceInfo.label, (VirtualEthernetCard)virtualDevice);
                }
            }
            return dictionary;
        }

        public bool IsPoweredOn()
        {
            return (VirtualMachinePowerState)this.GetProperties(new string[1] { "runtime.powerState" })["runtime.powerState"] == VirtualMachinePowerState.poweredOn;
        }

        public void PowerOn(VimClientlContext ctx)
        {
            ManagedObjectReference managedObject = this.VcService.Service.PowerOnVM_Task(this.ManagedObject, (ManagedObjectReference)null);
            ctx.IsRetriableCall = false;
            new Task(this.VcService, managedObject).WaitForResult("PowerOn", ctx);
        }

        public void PowerOff(VimClientlContext ctx)
        {
            ManagedObjectReference managedObject = this.VcService.Service.PowerOffVM_Task(this.ManagedObject);
            ctx.IsRetriableCall = false;
            new Task(this.VcService, managedObject).WaitForResult("PowerOff", ctx);
        }

        public void ShutdownGuestOS(VimClientlContext ctx)
        {
            try
            {
                this.VcService.Service.ShutdownGuest(this.ManagedObject);
            }
            catch (Exception ex)
            {
                this.PowerOff(ctx);
            }
        }

        public void RebootGuestOS(VimClientlContext ctx)
        {
            this.VcService.Service.RebootGuest(this.ManagedObject);
        }

        public IVimHost GetHost(ManagedObjectAndProperties[] data)
        {
            IVimHost vimHost = (IVimHost)null;
            foreach (ManagedObjectAndProperties objectAndProperties in data)
            {
                if (!(objectAndProperties.ManagedObject.type != "HostSystem") && objectAndProperties.Properties != null)
                {
                    vimHost = (IVimHost)new Host(this.VcService, objectAndProperties.ManagedObject);
                    vimHost.GetCommonProperties(objectAndProperties.Properties);
                    break;
                }
            }
            return vimHost;
        }

        public IVimHost GetHostWithoutProperties()
        {
            ManagedObjectReference[] managedObjects = this.GetManagedObjects(new string[1] { "summary.runtime.host" });
            IVimHost vimHost = (IVimHost)null;
            ManagedObjectReference[] managedObjectReferenceArray = managedObjects;
            int index = 0;
            if (index < managedObjectReferenceArray.Length)
                vimHost = (IVimHost)new Host(this.VcService, managedObjectReferenceArray[index]);
            return vimHost;
        }

        public IVimHost GetHostAndProperties()
        {
            return this.GetHost(this.GetManagedObjectAndProperties(this.ManagedObject, "summary.runtime.host", "HostSystem", Host.VCProperties));
        }

        public VmdkProperties[] GetActiveDiskFilesLayout()
        {
            VmdkProperties[] vmdkInfo = this.GetVMDKInfo();
            IVimDatastore[] datastoresAndProperties = this.GetDatastoresAndProperties();
            foreach (VmdkProperties vmdkProperties in vmdkInfo)
                vmdkProperties.FileName = this.ReplaceDatastoreName(datastoresAndProperties, vmdkProperties.FileName);
            return vmdkInfo;
        }

        private List<string[]> GetSnapshotLayouts()
        {
            List<string[]> strArrayList = new List<string[]>();
            VirtualMachineFileLayoutSnapshotLayout[] layoutSnapshotLayoutArray = (VirtualMachineFileLayoutSnapshotLayout[])null;
            Dictionary<string, object> properties = this.GetProperties(new string[1] { "layout.snapshot" });
            if (properties.ContainsKey("layout.snapshot"))
                layoutSnapshotLayoutArray = (VirtualMachineFileLayoutSnapshotLayout[])properties["layout.snapshot"];
            if (layoutSnapshotLayoutArray != null)
            {
                foreach (VirtualMachineFileLayoutSnapshotLayout layoutSnapshotLayout in layoutSnapshotLayoutArray)
                    strArrayList.Add(layoutSnapshotLayout.snapshotFile);
            }
            return strArrayList;
        }

        public string ReplaceDatastoreName(IVimDatastore[] datastores, string file)
        {
            string[] strArray = file.Split(new char[2] { '[', ']' });
            string str = (string)null;
            foreach (Datastore datastore in datastores)
            {
                if (datastore.DsProperties.Name == strArray[1])
                {
                    str = "/vmfs/volumes/" + datastore.DsProperties.RemoteId + "/" + strArray[2].Trim();
                    break;
                }
            }
            return str;
        }

        private static SnapshotsSummary ParseVmsd(string vmsdFile)
        {
            string str1 = (string)null;
            SnapshotsSummary snapshotsSummary = new SnapshotsSummary();
            Dictionary<string, VMSDEntry> dictionary1 = new Dictionary<string, VMSDEntry>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            string[] strArray1 = vmsdFile.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str2 in strArray1)
            {
                string[] separator = new string[1] { "=" };
                int num = 1;
                string[] strArray2 = str2.Split(separator, (StringSplitOptions)num);
                if (strArray2 != null && strArray2.Length >= 2)
                {
                    string str3 = strArray2[0].Trim();
                    string s = strArray2[1].Trim().TrimStart('"').TrimEnd('"');
                    string[] strArray3 = str3.Split('.');
                    if (strArray3[0] == "snapshot")
                    {
                        if (strArray3[1] == "numSnapshots")
                            snapshotsSummary.NumOfSnapshots = int.Parse(s);
                        if (strArray3[1] == "current")
                            str1 = s;
                    }
                }
            }
            foreach (string str2 in strArray1)
            {
                string[] separator = new string[1] { "=" };
                int num = 1;
                string[] strArray2 = str2.Split(separator, (StringSplitOptions)num);
                if (strArray2 != null && strArray2.Length >= 2)
                {
                    string str3 = strArray2[0].Trim();
                    string s = strArray2[1].Trim().TrimStart('"').TrimEnd('"');
                    string[] strArray3 = str3.Split('.');
                    int result;
                    if (strArray3[0].StartsWith("snapshot") && int.TryParse(strArray3[0].Substring("snapshot".Length), out result))
                    {
                        if (!dictionary1.ContainsKey(strArray3[0]))
                        {
                            if (dictionary1.Count < snapshotsSummary.NumOfSnapshots)
                                dictionary1.Add(strArray3[0], new VMSDEntry()
                                {
                                    SnapshotN = strArray3[0]
                                });
                            else
                                continue;
                        }
                        if (strArray3[1] == "createTimeHigh")
                            dictionary1[strArray3[0]]._HighCreateTime = int.Parse(s);
                        else if (strArray3[1] == "createTimeLow")
                            dictionary1[strArray3[0]]._LowCreateTime = int.Parse(s);
                        else if (strArray3[1] == "filename")
                            dictionary1[strArray3[0]].Vmsn = s;
                        else if (strArray3[1] == "uid")
                            dictionary1[strArray3[0]].Uid = s;
                        else if (strArray3[1] == "displayName")
                            dictionary1[strArray3[0]].DisplayName = s;
                        else if (strArray3[1] == "parent")
                            dictionary1[strArray3[0]].ParentUid = s;
                    }
                }
            }
            Dictionary<DateTime, VMSDEntry> dictionary2 = new Dictionary<DateTime, VMSDEntry>();
            foreach (VMSDEntry vmsdEntry in dictionary1.Values)
            {
                if (!dictionary2.ContainsKey(vmsdEntry.CreateTime))
                    dictionary2.Add(vmsdEntry.CreateTime, vmsdEntry);
            }
            foreach (VMSDEntry vmsdEntry in dictionary2.Values)
            {
                if (vmsdEntry.Uid == str1)
                {
                    snapshotsSummary.CreateTimeOfCurrent = vmsdEntry.CreateTime;
                    break;
                }
            }
            snapshotsSummary.VmsdEntries = dictionary2;
            return snapshotsSummary;
        }

        private Dictionary<string, List<string>> GetVmsnAndVmdkInfo()
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            foreach (string[] snapshotLayout in this.GetSnapshotLayouts())
            {
                List<string> stringList = new List<string>();
                foreach (string str1 in snapshotLayout)
                {
                    string str2 = "/";
                    int startIndex = str1.LastIndexOf(str2) + 1;
                    string key = str1.Substring(startIndex);
                    if (key.EndsWith("vmsn"))
                    {
                        if (!dictionary.ContainsKey(key))
                            dictionary.Add(key, stringList);
                    }
                    else if (key.EndsWith("vmdk"))
                        stringList.Add(key);
                }
            }
            return dictionary;
        }

        public SnapshotsSummary GetSnapshotsSummary(string vmsdFile)
        {
            try
            {
                this._maxSnapshotDelta = (int)new AppSettingsReader().GetValue("MaxSnapshotDelta", typeof(int));
            }
            catch (Exception ex)
            {
            }
            this._logger.FormatInformationWithCategory("vm", "Allowable snapshot delta is: {0}", (object)this._maxSnapshotDelta, (object)"Vm");
            SnapshotsSummary vmsd = Vm.ParseVmsd(vmsdFile);
            Dictionary<string, List<string>> vmsnAndVmdkInfo = this.GetVmsnAndVmdkInfo();
            List<SnapshotNode> snapshotNodeList = new List<SnapshotNode>();
            VirtualMachineSnapshotInfo snapshotInfo = this.GetSnapshotInfo();
            if (snapshotInfo != null && snapshotInfo.rootSnapshotList != null && snapshotInfo.rootSnapshotList.Length != 0)
            {
                foreach (VirtualMachineSnapshotTree rootSnapshot in snapshotInfo.rootSnapshotList)
                {
                    SnapshotNode node = new SnapshotNode();
                    this.GetSnapshotTree(node, rootSnapshot, vmsd, vmsnAndVmdkInfo);
                    snapshotNodeList.Add(node);
                }
            }
            vmsd.Roots = snapshotNodeList.ToArray();
            return vmsd;
        }

        private void GetSnapshotTree(SnapshotNode node, VirtualMachineSnapshotTree branch, SnapshotsSummary snapshotsSummary, Dictionary<string, List<string>> vmsnVmdkInfo)
        {
            if (branch == null)
                return;
            this.ProcessSnapshotNode(node, branch, snapshotsSummary, vmsnVmdkInfo);
            if (branch.childSnapshotList == null)
                return;
            foreach (VirtualMachineSnapshotTree childSnapshot in branch.childSnapshotList)
            {
                SnapshotNode node1 = new SnapshotNode();
                this.ProcessSnapshotNode(node1, childSnapshot, snapshotsSummary, vmsnVmdkInfo);
                node.Children.Add(node1);
                this.GetSnapshotTree(node1, childSnapshot, snapshotsSummary, vmsnVmdkInfo);
            }
        }

        private void ProcessSnapshotNode(SnapshotNode node, VirtualMachineSnapshotTree branch, SnapshotsSummary snapshotsSummary, Dictionary<string, List<string>> vmsnVmdkInfo)
        {
            node.Description = branch.description;
            node.CreateTime = branch.createTime.ToUniversalTime();
            node.Mor = branch.snapshot;
            if (Math.Abs(node.CreateTime.Subtract(snapshotsSummary.CreateTimeOfCurrent).TotalMilliseconds) <= (double)this._maxSnapshotDelta)
                snapshotsSummary.Current = node;
            foreach (DateTime key in snapshotsSummary.VmsdEntries.Keys)
            {
                TimeSpan timeSpan = node.CreateTime.Subtract(key);
                if (Math.Abs(timeSpan.TotalMilliseconds) <= (double)this._maxSnapshotDelta)
                {
                    node.DisplayName = snapshotsSummary.VmsdEntries[key].DisplayName;
                    node.Vmsn = snapshotsSummary.VmsdEntries[key].Vmsn;
                    break;
                }
                this._logger.FormatInformationWithCategory("Vm", "Snapshot delta exceeded. Current delta is: {0}", (object)"Vm", (object)Math.Abs(timeSpan.TotalMilliseconds));
            }
            if (string.IsNullOrEmpty(node.Vmsn) || !vmsnVmdkInfo.ContainsKey(node.Vmsn))
                return;
            node.Vmdks = vmsnVmdkInfo[node.Vmsn];
        }

        public IVimTask[] GetRecentTasks()
        {
            ManagedObjectAndProperties[] objectAndProperties1 = this.GetManagedObjectAndProperties(this.ManagedObject, "recentTask", "Task", new string[3] { "info.completeTime", "info.descriptionId", "info.state" });
            List<IVimTask> vimTaskList = new List<IVimTask>();
            DateTime? completeTime = new DateTime?();
            foreach (ManagedObjectAndProperties objectAndProperties2 in objectAndProperties1)
            {
                if (objectAndProperties2.Properties != null)
                {
                    string property1 = (string)objectAndProperties2.Properties["info.descriptionId"];
                    completeTime = !objectAndProperties2.Properties.ContainsKey("info.completeTime") ? new DateTime?() : new DateTime?((DateTime)objectAndProperties2.Properties["info.completeTime"]);
                    TaskInfoState property2 = (TaskInfoState)objectAndProperties2.Properties["info.state"];
                    IVimTask vimTask = (IVimTask)new Task(this.VcService, objectAndProperties2.ManagedObject, property1, completeTime, property2);
                    vimTaskList.Add(vimTask);
                }
            }
            return vimTaskList.ToArray();
        }

        public Dictionary<string, long> GetVirtualDiskSize()
        {
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            Dictionary<string, long> dictionary = new Dictionary<string, long>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualDisk && !dictionary.ContainsKey(virtualDevice.deviceInfo.label))
                    dictionary.Add(virtualDevice.deviceInfo.label, ((VirtualDisk)virtualDevice).capacityInKB);
            }
            return dictionary;
        }

        public void AddVirtualDisks(VmDiskInfo[] diskInfos, VimClientlContext ctx)
        {
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            foreach (VmDiskInfo diskInfo in diskInfos)
            {
                VirtualDiskFlatVer2BackingInfo flatVer2BackingInfo = new VirtualDiskFlatVer2BackingInfo();
                flatVer2BackingInfo.fileName = diskInfo.File;
                flatVer2BackingInfo.diskMode = diskInfo.Mode;
                VirtualDisk virtualDisk = new VirtualDisk();
                virtualDisk.backing = (VirtualDeviceBackingInfo)flatVer2BackingInfo;
                virtualDisk.key = -1;
                virtualDisk.controllerKey = diskInfo.CtrlKey;
                virtualDisk.controllerKeySpecified = true;
                virtualDisk.unitNumber = diskInfo.UnitNumber;
                virtualDisk.unitNumberSpecified = true;
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    operation = VirtualDeviceConfigSpecOperation.add,
                    operationSpecified = true,
                    device = (VirtualDevice)virtualDisk
                });
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("AddVirtualDisks", ctx);
            string str = (string)null;
            for (int index = 0; index < 36; ++index)
            {
                string[] array = ((IEnumerable<VmdkProperties>)this.GetVMDKInfo()).Select<VmdkProperties, string>((Func<VmdkProperties, string>)(info => info.FileName)).ToArray<string>();
                str = (string)null;
                foreach (VmDiskInfo diskInfo in diskInfos)
                {
                    if (!((IEnumerable<string>)array).Contains<string>(diskInfo.File, (IEqualityComparer<string>)new Vm.VMWareFilenameEqualityComparer()))
                    {
                        str = diskInfo.File;
                        this._logger.Verbose("We were looking for the file " + str + ".");
                        this._logger.FormatVerbose("The files currently attached are: {0}.", (object)string.Join(", ", array));
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(str))
                    Thread.Sleep(5000);
                else
                    break;
            }
            if (!string.IsNullOrEmpty(str))
                throw new ApplicationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Virtual disk {0} has not been added.", new object[1] { (object)str }));
        }

        public void AttachVirtualDisks(IVimVm vm, VimClientlContext ctx)
        {
            VmdkProperties[] vmdkInfo = vm.GetVMDKInfo();
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            List<ScsiInfo> availableScsiSlots = this.GetAvailableScsiSlots();
            int index1 = 0;
            foreach (VmdkProperties vmdkProperties in vmdkInfo)
            {
                if (availableScsiSlots.Count == index1)
                    throw new OculiServiceServiceException(0, "Not enough slots available to attach drives");
                VirtualDiskFlatVer2BackingInfo flatVer2BackingInfo = new VirtualDiskFlatVer2BackingInfo();
                flatVer2BackingInfo.fileName = vmdkProperties.FileName;
                flatVer2BackingInfo.diskMode = vmdkProperties.DiskMode;
                VirtualDisk virtualDisk = new VirtualDisk();
                virtualDisk.backing = (VirtualDeviceBackingInfo)flatVer2BackingInfo;
                virtualDisk.key = -1;
                virtualDisk.controllerKey = availableScsiSlots[index1].CtrlKey;
                virtualDisk.controllerKeySpecified = true;
                virtualDisk.unitNumber = availableScsiSlots[index1].Unit;
                virtualDisk.unitNumberSpecified = true;
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    operation = VirtualDeviceConfigSpecOperation.add,
                    operationSpecified = true,
                    device = (VirtualDevice)virtualDisk
                });
                ++index1;
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("AddVirtualDisks", ctx);
            string str = (string)null;
            for (int index2 = 0; index2 < 36; ++index2)
            {
                str = (string)null;
                Dictionary<string, string> virtualDiskLabels = this.GetVirtualDiskLabels();
                foreach (VmdkProperties vmdkProperties in vmdkInfo)
                {
                    if (!virtualDiskLabels.ContainsKey(vmdkProperties.FileName))
                    {
                        str = vmdkProperties.FileName;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(str))
                    Thread.Sleep(5000);
                else
                    break;
            }
            if (!string.IsNullOrEmpty(str))
                throw new ApplicationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Virtual disk {0} has not been attached.", new object[1] { (object)str }));
        }

        private List<ScsiInfo> GetAvailableScsiSlots()
        {
            List<ScsiInfo> scsiInfoList = new List<ScsiInfo>();
            Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
            foreach (ScsiControllerInfo scsiControllerInfo in this.GetScsiControllersInfo())
            {
                List<int> intList = new List<int>(15);
                for (int index = 0; index < 16; ++index)
                {
                    if (index != 7)
                        intList.Add(index);
                }
                dictionary[scsiControllerInfo.CtrlKey] = intList;
            }
            foreach (VmdkProperties vmdkProperties in this.GetVMDKInfo())
                dictionary[vmdkProperties.ControllerKey].Remove(vmdkProperties.UnitNumber);
            foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
            {
                foreach (int unit in keyValuePair.Value)
                    scsiInfoList.Add(new ScsiInfo(keyValuePair.Key, unit));
            }
            return scsiInfoList;
        }

        private VirtualDisk[] GetVirtualDisks()
        {
            VirtualDevice[] property = (VirtualDevice[])this.GetProperties(new string[1] { "config.hardware.device" })["config.hardware.device"];
            List<VirtualDisk> virtualDiskList = new List<VirtualDisk>();
            foreach (VirtualDevice virtualDevice in property)
            {
                if (virtualDevice is VirtualDisk)
                    virtualDiskList.Add(virtualDevice as VirtualDisk);
            }
            return virtualDiskList.ToArray();
        }

        public void DetachVirtualDisks(IVimVm vm, VimClientlContext ctx)
        {
            VmdkProperties[] vmdkInfo = vm.GetVMDKInfo();
            Dictionary<string, string> sourceVmdksDict = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            System.Action<VmdkProperties> action = (System.Action<VmdkProperties>)(v => sourceVmdksDict.Add(v.FileName, v.FileName));
            ((IEnumerable<VmdkProperties>)vmdkInfo).ForEach<VmdkProperties>(action);
            VirtualDisk[] virtualDisks = this.GetVirtualDisks();
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            foreach (VirtualDisk virtualDisk in virtualDisks)
            {
                string fileName = ((VirtualDeviceFileBackingInfo)virtualDisk.backing).fileName;
                if (sourceVmdksDict.ContainsKey(fileName))
                    deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                    {
                        device = (VirtualDevice)virtualDisk,
                        operation = VirtualDeviceConfigSpecOperation.remove,
                        operationSpecified = true
                    });
            }
            VirtualMachineConfigSpec spec = new VirtualMachineConfigSpec();
            spec.deviceChange = deviceConfigSpecList.ToArray();
            ctx.IsRetriableCall = false;
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, spec)).WaitForResult("ReconfigVM", ctx);
        }

        public void CreateVirtualDisks(VmDiskInfo[] diskInfos, VimClientlContext ctx)
        {
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            int num = -1;
            foreach (VmDiskInfo diskInfo in diskInfos)
            {
                string str = this.GetVolumeName(diskInfo.Location.Name);
                if (!string.IsNullOrEmpty(diskInfo.File))
                    str = str + " " + this.Name + "/" + diskInfo.File;
                IVimDatastore datastoreByName = this.VcService.GetDatastoreByName(diskInfo.Location.Name);
                if (!datastoreByName.DirectoryExist(this.Name, ctx))
                    datastoreByName.CreateDirectory(this.Name);
                VirtualDiskFlatVer2BackingInfo flatVer2BackingInfo = new VirtualDiskFlatVer2BackingInfo();
                flatVer2BackingInfo.datastore = diskInfo.Location.ManagedObject;
                flatVer2BackingInfo.fileName = str;
                flatVer2BackingInfo.diskMode = diskInfo.Mode;
                if (diskInfo.DiskType == "Flat Disk")
                {
                    flatVer2BackingInfo.eagerlyScrub = false;
                    flatVer2BackingInfo.eagerlyScrubSpecified = true;
                    flatVer2BackingInfo.thinProvisioned = false;
                    flatVer2BackingInfo.thinProvisionedSpecified = true;
                }
                else if (diskInfo.DiskType == "Dynamic")
                {
                    flatVer2BackingInfo.thinProvisioned = true;
                    flatVer2BackingInfo.thinProvisionedSpecified = true;
                }
                else if (diskInfo.DiskType == "Fixed")
                {
                    flatVer2BackingInfo.eagerlyScrub = true;
                    flatVer2BackingInfo.eagerlyScrubSpecified = true;
                }
                VirtualDisk virtualDisk = new VirtualDisk();
                virtualDisk.key = num;
                virtualDisk.controllerKey = diskInfo.CtrlKey;
                virtualDisk.controllerKeySpecified = true;
                virtualDisk.unitNumber = diskInfo.UnitNumber;
                virtualDisk.unitNumberSpecified = true;
                virtualDisk.backing = (VirtualDeviceBackingInfo)flatVer2BackingInfo;
                virtualDisk.capacityInKB = diskInfo.SizeMB * 1024L;
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    fileOperation = VirtualDeviceConfigSpecFileOperation.create,
                    fileOperationSpecified = true,
                    operation = VirtualDeviceConfigSpecOperation.add,
                    operationSpecified = true,
                    device = (VirtualDevice)virtualDisk
                });
                --num;
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("Create virtual disks", ctx);
        }

        public void CloneVm(string targetVmName, IVimHost targetEsx, IVimDatastore targetDatastore, VimClientlContext ctx)
        {
            //string dateCreated = (System.DateTime.Now).ToString();
            //Random rand = new Random();
            //if (txtVmName.Text == null || txtVmName.Text == "")
            //{
            //    txtResult.Text = "Please enter a name for the virtual machine.";
            //    return;
            //}

            //IPAddress theIp;
            //bool ipResult = IPAddress.TryParse(txtIpAddress.Text, out theIp);
            //if (ipResult != true)
            //{
            //    txtResult.Text = "Please enter a valid IP Address.";
            //    return;
            //}

            //IPAddress theGateway;
            //bool gwResult = IPAddress.TryParse(txtDefaultGateway.Text, out theGateway);
            //if (gwResult != true)
            //{
            //    txtResult.Text = "Please entera valid IP Address for the default gateway.";
            //    return;
            //}
            ////  
            //// Check for name conflict  
            ////  

            //VimClient vimClient = ConnectServer(Globals.sViServer, Globals.sUsername, Globals.sPassword);
            //List<VirtualMachine> chkVirtualMachines = GetVirtualMachines(vimClient, null, txtVmName.Text);
            //if (chkVirtualMachines != null)
            //{
            //    vimClient.Disconnect();
            //    txtResult.Text = "virtual machine " + txtVmName.Text + " already exists";
            //    return;
            //}
            //if (cboCustomizations.SelectedItem.Value == "Windows")
            //{
            //    //  
            //    // Connect to directory and create computer object  
            //    //  
            //    string ldapPath = Globals.adServer + "/OU=" + cboSysadminOu.SelectedItem.ToString() + "," + Globals.adRootPath;
            //    DirectoryEntry dirEntry = new DirectoryEntry();
            //    dirEntry.Username = Globals.sUsername;
            //    dirEntry.Password = Globals.sPassword;
            //    //  
            //    // check to see if name exists  
            //    //  

            //    DirectorySearcher dirSearcher = new DirectorySearcher(dirEntry);
            //    dirSearcher.Filter = "(cn=" + txtVmName.Text + ")";
            //    SearchResult dirResult = dirSearcher.FindOne();
            //    if (dirResult == null)
            //    {
            //        //  
            //        // Name not found, create new ad object  
            //        //  
            //        DirectoryEntry dirEntry1 = new DirectoryEntry();
            //        dirEntry1.Path = ldapPath;
            //        dirEntry1.Username = Globals.sUsername;
            //        dirEntry1.Password = Globals.sPassword;
            //        DirectoryEntries dirEntries = dirEntry1.Children;
            //        DirectoryEntry newVm = dirEntries.Add("CN=" + txtVmName.Text.ToString(), "computer");
            //        newVm.Properties["sAMAccountName"].Value = txtVmName.Text.ToString() + "$";
            //        newVm.Properties["Description"].Value = Globals.sUsername + " created this object on " + DateTime.Now;
            //        newVm.Properties["userAccountControl"].Value = 4128;
            //        newVm.CommitChanges();
            //    }
            //    else
            //    {
            //        txtResult.Text = "The server " + txtVmName.Text + " already exists in the Active Directory, please choose a different name.";
            //        return;
            //    }
            //}
            ////  
            //// Get a list of hosts in the selected cluster  
            ////  
            //List<HostSystem> lstHosts = GetHosts(vimClient, cboClusters.SelectedValue);
            ////  
            //// Randomly pick host  
            ////  

            //HostSystem selectedHost = lstHosts[rand.Next(0, lstHosts.Count)];

            ////  
            //// Connect to selected vm to clone  
            ////  

            //List<VirtualMachine> lstVirtualMachines = GetVirtualMachines(vimClient, null, cboClones.SelectedItem.Text);
            //VirtualMachine itmVirtualMachine = lstVirtualMachines[0];
            ////  
            //// Connect to the selected datastore  
            ////  

            //List<Datastore> lstDatastores = GetDataStore(vimClient, null, cboDatastores.SelectedItem.Text);
            //Datastore itmDatastore = lstDatastores[0];
            ////  
            //// Connect to portgroup  
            ////  
            //List<DistributedVirtualPortgroup> lstDvPortGroups = GetDVPortGroups(vimClient, null, cboPortGroups.SelectedItem.Text);
            //DistributedVirtualPortgroup itmDvPortGroup = lstDvPortGroups[0]; ;

            ////  
            //// Connect to the customizationspec  
            ////  
            //CustomizationSpecItem itmSpecItem = GetCustomizationSpecItem(vimClient, cboCustomizations.SelectedItem.Text);
            ////  
            //// Create a new VirtualMachineCloneSpec  
            ////  

            //VirtualMachineCloneSpec mySpec = new VirtualMachineCloneSpec();
            //mySpec.Location = new VirtualMachineRelocateSpec();
            //mySpec.Location.Datastore = itmDatastore.MoRef;
            //mySpec.Location.Host = selectedHost.MoRef;

            ////  
            //// Add selected CloneSpec customizations to this CloneSpec  
            ////  

            //mySpec.Customization = itmSpecItem.Spec;
            ////  
            //// Handle hostname for either windows or linux  
            ////  
            //if (cboCustomizations.SelectedValue == "Windows")
            //{
            //    //  
            //    // Create a windows sysprep object  
            //    //  
            //    CustomizationSysprep winIdent = (CustomizationSysprep)itmSpecItem.Spec.Identity;
            //    CustomizationFixedName hostname = new CustomizationFixedName();
            //    hostname.Name = txtVmName.Text;
            //    winIdent.UserData.ComputerName = hostname;
            //    //  
            //    // Store identity in this CloneSpec  
            //    //  
            //    mySpec.Customization.Identity = winIdent;
            //}
            //if (cboCustomizations.SelectedValue == "Linux")
            //{
            //    //  
            //    // Create a Linux "sysprep" object  
            //    //  
            //    CustomizationLinuxPrep linIdent = (CustomizationLinuxPrep)itmSpecItem.Spec.Identity;
            //    CustomizationFixedName hostname = new CustomizationFixedName();
            //    hostname.Name = txtVmName.Text;
            //    linIdent.HostName = hostname;
            //    //  
            //    // Store identity in this CloneSpec  
            //    //  
            //    mySpec.Customization.Identity = linIdent;
            //}
            ////  
            //// Create a new ConfigSpec  
            ////  
            //mySpec.Config = new VirtualMachineConfigSpec();
            ////  
            //// Set number of CPU's  
            ////  

            //int numCpus = new int();
            //numCpus = Convert.ToInt16(cboCpuNum.SelectedValue);
            //mySpec.Config.NumCPUs = numCpus;
            ////  
            //// Set amount of RAM  
            ////  

            //long memoryMb = new long();
            //memoryMb = (long)(Convert.ToInt16(cboRam.SelectedValue) * 1024);
            //mySpec.Config.MemoryMB = memoryMb;
            ////  
            //// Only handle the first network card  
            ////  

            //mySpec.Customization.NicSettingMap = new CustomizationAdapterMapping[1];
            //mySpec.Customization.NicSettingMap[0] = new CustomizationAdapterMapping();

            ////  
            //// Read in the DNS from web.config and assign  
            ////  

            //string[] ipDns = new string[1];
            //ipDns[0] = WebConfigurationManager.AppSettings["dnsServer"].ToString();
            //mySpec.Customization.GlobalIPSettings = new CustomizationGlobalIPSettings();
            //mySpec.Customization.GlobalIPSettings.DnsServerList = ipDns;
            ////  
            //// Create a new networkDevice  
            ////  

            //VirtualDevice networkDevice = new VirtualDevice();
            //foreach (VirtualDevice vDevice in itmVirtualMachine.Config.Hardware.Device)
            //{
            //    //  
            //    // get nic on vm  
            //    //  
            //    if (vDevice.DeviceInfo.Label.Contains("Network"))
            //    {
            //        networkDevice = vDevice;
            //    }
            //}
            ////  
            //// Create a DeviceSpec  
            ////  

            //VirtualDeviceConfigSpec[] devSpec = new VirtualDeviceConfigSpec[0];
            //mySpec.Config.DeviceChange = new VirtualDeviceConfigSpec[1];
            //mySpec.Config.DeviceChange[0] = new VirtualDeviceConfigSpec();
            //mySpec.Config.DeviceChange[0].Operation = VirtualDeviceConfigSpecOperation.edit;
            //mySpec.Config.DeviceChange[0].Device = networkDevice;
            ////  
            //// Define network settings for the new vm  
            ////  

            //CustomizationFixedIp ipAddress = new CustomizationFixedIp();
            //ipAddress.IpAddress = txtIpAddress.Text;
            //mySpec.Customization.NicSettingMap[0].Adapter = new CustomizationIPSettings();

            ////  
            //// Assign IP address  
            ////  
            //mySpec.Customization.NicSettingMap[0].Adapter.Ip = ipAddress;
            //mySpec.Customization.NicSettingMap[0].Adapter.SubnetMask = txtSubnetMask.Text;

            ////  
            //// Assign default gateway  
            ////  

            //string[] ipGateway = new string[1];
            //ipGateway[0] = txtDefaultGateway.Text;
            //mySpec.Customization.NicSettingMap[0].Adapter.Gateway = ipGateway;
            ////  
            //// Create network backing information  
            ////  

            //VirtualEthernetCardDistributedVirtualPortBackingInfo nicBack = new VirtualEthernetCardDistributedVirtualPortBackingInfo();
            //nicBack.Port = new DistributedVirtualSwitchPortConnection();
            ////  
            //// Connect to the virtual switch  
            ////  

            //VmwareDistributedVirtualSwitch dvSwitch = GetDvSwitch(vimClient, itmDvPortGroup.Config.DistributedVirtualSwitch);
            ////  
            //// Assign the proper switch port  
            ////  

            //nicBack.Port.SwitchUuid = dvSwitch.Uuid;
            ////  
            //// Connect the network card to proper port group  
            ////  

            //nicBack.Port.PortgroupKey = itmDvPortGroup.MoRef.Value;
            //mySpec.Config.DeviceChange[0].Device.Backing = nicBack;
            ////  
            //// Enable the network card at bootup  
            ////  

            //mySpec.Config.DeviceChange[0].Device.Connectable = new VirtualDeviceConnectInfo();
            //mySpec.Config.DeviceChange[0].Device.Connectable.StartConnected = true;
            //mySpec.Config.DeviceChange[0].Device.Connectable.AllowGuestControl = true;
            //mySpec.Config.DeviceChange[0].Device.Connectable.Connected = true;
            ////  
            //// Get the vmfolder from the datacenter  
            ////  

            //List<ClusterComputeResource> lstClusters = GetClusters(vimClient, cboClusters.SelectedItem.Text);
            //List<Datacenter> lstDatacenters = GetDcFromCluster(vimClient, lstClusters[0].Parent.Value);
            //Datacenter itmDatacenter = lstDatacenters[0];

            ////  
            //// Perform the clone  
            ////  
            //itmVirtualMachine.CloneVM_Task(itmDatacenter.VmFolder, txtVmName.Text, mySpec);

            //vimClient.Disconnect();
            //VirtualMachineCloneSpec vmCloneSpec = this.CreateVmCloneSpec(targetEsx, targetDatastore);
            //vmCloneSpec.powerOn = false;
            //vmCloneSpec.template = false;
            //new Task(this.VcService, this.VcService.Service.CloneVM_Task(this.ManagedObject, targetEsx.GetDatacenterAndProperties().GetVmFolder(), targetVmName, vmCloneSpec)).WaitForResult("Deploy From Template/Clone", ctx);
        }

        private VirtualMachineCloneSpec CreateVmCloneSpec(IVimHost targetEsx, IVimDatastore targetDatastore)
        {
            VirtualMachineCloneSpec machineCloneSpec = new VirtualMachineCloneSpec();
            VirtualMachineRelocateSpec machineRelocateSpec1 = new VirtualMachineRelocateSpec();
            IVimResourcePool defaultResourcePool = targetEsx.GetDefaultResourcePool();
            machineRelocateSpec1.pool = defaultResourcePool.ManagedObject;
            machineRelocateSpec1.datastore = targetDatastore.ManagedObject;
            VirtualMachineRelocateSpec machineRelocateSpec2 = machineRelocateSpec1;
            machineCloneSpec.location = machineRelocateSpec2;
            return machineCloneSpec;
        }

        public void CloneVm(string targetVmName, IVimHost targetEsx, IVimDatastore targetDatastore, IVimResourcePool targetResourcePool, string customizationSpecName, string[] ipAddresses, VimClientlContext ctx)
        {
            VirtualMachineCloneSpec vmCloneSpec = this.CreateVmCloneSpec(targetEsx, targetDatastore, targetResourcePool, customizationSpecName, ipAddresses);
            new Task(this.VcService, this.VcService.Service.CloneVM_Task(this.ManagedObject, targetEsx.GetDatacenterAndProperties().GetVmFolder(), targetVmName, vmCloneSpec)).WaitForResult("Clone VM/Deploy From Template", ctx);
        }

        private VirtualMachineCloneSpec CreateVmCloneSpec(IVimHost targetEsx, IVimDatastore targetDatastore, IVimResourcePool targetResourcePool, string custSpecName, string[] ipAddresses)
        {
            CustomizationSpecItem customizationSpecItem = (CustomizationSpecItem)null;
            if (!string.IsNullOrEmpty(custSpecName))
            {
                customizationSpecItem = this.VcService.GetCustomizationSpec(custSpecName);
                if (customizationSpecItem != null && ipAddresses != null && ipAddresses.Length != 0)
                {
                    List<string> stringList = new List<string>();
                    foreach (string ipAddress in ipAddresses)
                    {
                        IPAddress address;
                        if (IPAddress.TryParse(ipAddress, out address))
                            stringList.Add(ipAddress);
                    }
                    if (stringList.Count > 0)
                    {
                        int index = 0;
                        foreach (CustomizationAdapterMapping nicSetting in customizationSpecItem.spec.nicSettingMap)
                        {
                            if (stringList.Count > index)
                            {
                                nicSetting.adapter.ip = (CustomizationIpGenerator)new CustomizationFixedIp()
                                {
                                    ipAddress = stringList[index]
                                };
                                ++index;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            VirtualMachineCloneSpec machineCloneSpec = new VirtualMachineCloneSpec();
            machineCloneSpec.location = new VirtualMachineRelocateSpec();
            if (targetResourcePool == null)
                targetResourcePool = targetEsx.GetDefaultResourcePool();
            machineCloneSpec.location.pool = targetResourcePool.ManagedObject;
            machineCloneSpec.location.datastore = targetDatastore.ManagedObject;
            if (customizationSpecItem != null)
                machineCloneSpec.customization = customizationSpecItem.spec;
            machineCloneSpec.powerOn = true;
            machineCloneSpec.template = false;
            return machineCloneSpec;
        }

        public void MigrateVm(IVimHost targetHost)
        {
            this.VcService.Service.MigrateVM_Task(this.ManagedObject, targetHost.GetDefaultResourcePool().ManagedObject, targetHost.ManagedObject, VirtualMachineMovePriority.defaultPriority, VirtualMachinePowerState.poweredOff, false);
        }

        public Dictionary<string, string> GetVirtualDiskLabels()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            string[] properties = new string[1] { "config.hardware.device" };
            foreach (VirtualDevice virtualDevice in (VirtualDevice[])this.GetProperties(properties)["config.hardware.device"])
            {
                if (virtualDevice is VirtualDisk)
                {
                    string fileName = ((VirtualDeviceFileBackingInfo)virtualDevice.backing).fileName;
                    if (!dictionary.ContainsKey(fileName))
                        dictionary.Add(fileName, virtualDevice.deviceInfo.label);
                }
            }
            return dictionary;
        }

        public int GetNumOfPCIDevices()
        {
            int num = 0;
            string[] properties = new string[1] { "config.hardware.device" };
            foreach (VirtualDevice virtualDevice in (VirtualDevice[])this.GetProperties(properties)["config.hardware.device"])
            {
                if (virtualDevice is VirtualPCIController)
                {
                    num = ((VirtualController)virtualDevice).device.Length;
                    break;
                }
            }
            return num;
        }

        public ScsiControllerInfo[] GetScsiControllersInfo()
        {
            List<ScsiControllerInfo> scsiControllerInfoList = new List<ScsiControllerInfo>();
            string[] properties = new string[1] { "config.hardware.device" };
            foreach (VirtualDevice virtualDevice in (VirtualDevice[])this.GetProperties(properties)["config.hardware.device"])
            {
                if (virtualDevice is VirtualSCSIController)
                    scsiControllerInfoList.Add(new ScsiControllerInfo(virtualDevice.deviceInfo.summary, virtualDevice.key, ((VirtualController)virtualDevice).busNumber));
            }
            return scsiControllerInfoList.ToArray();
        }

        public VirtualMachineToolsStatus GetVMToolsStatus()
        {
            VirtualMachineToolsStatus machineToolsStatus = VirtualMachineToolsStatus.toolsNotInstalled;
            Dictionary<string, object> properties = this.GetProperties(new string[1] { "guest.toolsStatus" });
            if (properties["guest.toolsStatus"] != null)
                machineToolsStatus = (VirtualMachineToolsStatus)properties["guest.toolsStatus"];
            return machineToolsStatus;
        }

        public void UpgradeVMTools(VimClientlContext ctx)
        {
            new Task(this.VcService, this.VcService.Service.UpgradeTools_Task(this.ManagedObject, (string)null)).WaitForResult("Upgrade VMTools", ctx);
            Thread.Sleep(5000);
        }

        public void MountVMTools()
        {
            this.VcService.Service.MountToolsInstaller(this.ManagedObject);
            Thread.Sleep(5000);
        }

        public void UnmountVMTools()
        {
            this.VcService.Service.UnmountToolsInstaller(this.ManagedObject);
            Thread.Sleep(5000);
        }

        public bool HasSnapshotableDisks(VmdkProperties[] vmdkInfos)
        {
            bool flag = false;
            foreach (VmdkProperties vmdkInfo in vmdkInfos)
            {
                if (Vm.IsSnapshottableDisk(vmdkInfo.DiskMode))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public ManagedObjectAndProperties[] GetManagedObjectsAndProperties()
        {
            PropertySpec propertySpec1 = new PropertySpec();
            propertySpec1.type = "VirtualMachine";
            propertySpec1.all = false;
            propertySpec1.pathSet = Vm.VCProperties;
            PropertySpec propertySpec2 = new PropertySpec();
            propertySpec2.type = "HostSystem";
            propertySpec2.all = false;
            propertySpec2.pathSet = Host.VCProperties;
            PropertySpec propertySpec3 = new PropertySpec();
            propertySpec3.type = "Datastore";
            propertySpec3.all = false;
            propertySpec3.pathSet = Datastore.VCProperties;
            PropertySpec propertySpec4 = new PropertySpec();
            propertySpec4.type = "Network";
            propertySpec4.all = false;
            propertySpec4.pathSet = new string[1] { "name" };
            ObjectSpec objectSpec = new ObjectSpec();
            objectSpec.obj = this.ManagedObject;
            objectSpec.skip = false;
            objectSpec.selectSet = new SelectionSpec[3]
            {
        (SelectionSpec) new TraversalSpec()
        {
          type = "VirtualMachine",
          path = "summary.runtime.host"
        },
        (SelectionSpec) new TraversalSpec()
        {
          type = "VirtualMachine",
          path = "datastore"
        },
        (SelectionSpec) new TraversalSpec()
        {
          type = "VirtualMachine",
          path = "network"
        }
            };
            PropertyFilterSpec propertyFilterSpec = new PropertyFilterSpec();
            propertyFilterSpec.propSet = new PropertySpec[4]
            {
        propertySpec1,
        propertySpec2,
        propertySpec3,
        propertySpec4
            };
            propertyFilterSpec.objectSet = new ObjectSpec[1]
            {
        objectSpec
            };
            List<ManagedObjectAndProperties> objectAndPropertiesList = new List<ManagedObjectAndProperties>();
            ObjectContent[] objectContentArray = this.VcService.RetrieveProperties(new PropertyFilterSpec[1] { propertyFilterSpec });
            if (objectContentArray != null)
            {
                foreach (ObjectContent objectContent in objectContentArray)
                    objectAndPropertiesList.Add(new ManagedObjectAndProperties()
                    {
                        ManagedObject = objectContent.obj,
                        Properties = this.VcService.PropSetToDictionary(objectContent.propSet)
                    });
            }
            return objectAndPropertiesList.ToArray();
        }

        public bool IsNicChanged(FailoverConfig failoverConfig)
        {
            return this.GetEthernetCards().Count != failoverConfig.NicMappings.Length;
        }

        public bool IsVmdkChanged(ref VmxConfiguration srcCfg, IVimDatastore[] datastores, IVimHost host, GetBaseVmdksDelegate getBaseVmdks)
        {
            VmdkMapping[] vmdkMappings = srcCfg.VmdkMappings;
            Dictionary<int, VmdkMapping> dictionary = new Dictionary<int, VmdkMapping>();
            foreach (VmdkMapping vmdkMapping in vmdkMappings)
            {
                if (!dictionary.ContainsKey(vmdkMapping.Key))
                    dictionary.Add(vmdkMapping.Key, vmdkMapping);
            }
            srcCfg = this.CreateSourceVmdkMappings(srcCfg, datastores, host, getBaseVmdks);
            foreach (VmdkMapping vmdkMapping in srcCfg.VmdkMappings)
            {
                if (dictionary.ContainsKey(vmdkMapping.Key) && dictionary[vmdkMapping.Key].VmdkFullName == vmdkMapping.VmdkFullName)
                    vmdkMapping.TimeStamp = dictionary[vmdkMapping.Key].TimeStamp;
            }
            if (vmdkMappings.Length != srcCfg.VmdkMappings.Length)
                return true;
            foreach (VmdkMapping vmdkMapping in srcCfg.VmdkMappings)
            {
                if (!dictionary.ContainsKey(vmdkMapping.Key) || string.Compare(dictionary[vmdkMapping.Key].VmdkFullName, vmdkMapping.VmdkFullName, true, CultureInfo.InvariantCulture) != 0)
                    return true;
            }
            return false;
        }

        public VmxConfiguration CreateSourceVmdkMappings(VmxConfiguration vmxCgf, IVimDatastore[] datastores, IVimHost host, GetBaseVmdksDelegate getBaseVmdks)
        {
            VmxConfiguration vmxCfg = (VmxConfiguration)vmxCgf.ShallowCopy();
            List<VmdkMapping> vmdkMappingList = new List<VmdkMapping>();
            if (datastores == null || datastores.Length == 0)
                datastores = this.GetDatastoresAndProperties();
            foreach (VmdkProperties vmdkProperties in this.GetVMDKInfo(datastores))
            {
                VmdkMapping vmdkMapping = new VmdkMapping();
                vmdkMapping.IsSnapshotable = !vmdkProperties.DiskMode.Contains("independent");
                vmdkMapping.VmdkSize = vmdkProperties.Capacity * 1024L;
                vmdkMapping.Key = vmdkProperties.Key;
                vmdkMapping.Label = vmdkProperties.Label;
                vmdkMapping.VmdkFullName = !vmdkProperties.FileName.StartsWith("/vmfs") ? "/vmfs/volumes/" + vmdkProperties.FileName : vmdkProperties.FileName;
                vmdkMapping.VmdkFullName = this.ReplaceDatastoreName((IVimDatastore[])new Datastore[1]
                {
          (Datastore) vmdkProperties.Datastore
                }, vmdkMapping.VmdkFullName);
                vmdkMappingList.Add(vmdkMapping);
            }
            vmxCfg.VmdkMappings = vmdkMappingList.ToArray();
            VmxConfiguration vmxConfiguration = getBaseVmdks(vmxCfg);
            if (this.HasSnapshots())
            {
                if (host == null)
                    host = this.GetHostWithoutProperties();
                datastores = host.GetDatastoresAndProperties();
            }
            foreach (VmdkMapping vmdkMapping in vmxConfiguration.VmdkMappings)
                Vm.PopulateDatastoreUrlAndNameInVmdkMapping(vmdkMapping, datastores);
            foreach (VmdkMapping vmdkMapping in vmxConfiguration.VmdkMappings)
            {
                vmdkMapping.VmdkShortName = Path.GetFileName(vmdkMapping.VmdkFullName);
                vmdkMapping.VmdkDir = CUtils.GetDirectoryName(vmdkMapping.VmdkFullName);
                vmdkMapping.VmdkDir = vmdkMapping.VmdkDir.Replace('\\', '/');
                vmdkMapping.VmdkBaseName = Path.GetFileNameWithoutExtension(vmdkMapping.VmdkFullName);
                string extension = Path.GetExtension(vmdkMapping.VmdkFullName);
                vmdkMapping.VmdkShortNameBin = vmdkMapping.VmdkBaseName + "-flat" + extension;
                vmdkMapping.VmdkFullNameBin = vmdkMapping.VmdkDir + "/" + vmdkMapping.VmdkShortNameBin;
            }
            return vmxConfiguration;
        }

        private static void PopulateDatastoreUrlAndNameInVmdkMapping(VmdkMapping vmdkMapping, IVimDatastore[] datastores)
        {
            string[] strArray = vmdkMapping.VmdkFullName.Split('/');
            foreach (IVimDatastore datastore in datastores)
            {
                if (strArray.Length > 2 && strArray[3] == datastore.DsProperties.RemoteId)
                {
                    vmdkMapping.DatastoreUrl = datastore.DsProperties.Url;
                    vmdkMapping.DatastoreName = datastore.DsProperties.Name;
                    break;
                }
            }
        }

        public void AddScsiCtrl(int[] busNumbers, ScsiControllerType scsiControllerType, VimClientlContext ctx)
        {
            if (busNumbers == null || busNumbers.Length == 0)
                return;
            int num = 1;
            List<VirtualDeviceConfigSpec> deviceConfigSpecList = new List<VirtualDeviceConfigSpec>();
            for (int index = 0; index < busNumbers.Length; ++index)
            {
                VirtualSCSIController virtualScsiController = scsiControllerType != ScsiControllerType.LsiLogicSAS ? (scsiControllerType != ScsiControllerType.LsiLogicParallel ? (VirtualSCSIController)new VirtualBusLogicController() : (VirtualSCSIController)new VirtualLsiLogicController()) : (VirtualSCSIController)new VirtualLsiLogicSASController();
                virtualScsiController.busNumber = busNumbers[index];
                virtualScsiController.key = num;
                virtualScsiController.sharedBus = VirtualSCSISharing.noSharing;
                deviceConfigSpecList.Add(new VirtualDeviceConfigSpec()
                {
                    operation = VirtualDeviceConfigSpecOperation.add,
                    operationSpecified = true,
                    device = (VirtualDevice)virtualScsiController
                });
            }
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, new VirtualMachineConfigSpec()
            {
                deviceChange = deviceConfigSpecList.ToArray()
            })).WaitForResult("AddScsiCtrl", ctx);
        }

        public void Reconfigure(string op, VirtualMachineConfigSpec spec, VimClientlContext ctx)
        {
            new Task(this.VcService, this.VcService.Service.ReconfigVM_Task(this.ManagedObject, spec)).WaitForResult(op, ctx);
        }

        public IVimResourcePool GetResourcePool()
        {
            ManagedObjectAndProperties[] objectAndProperties1 = this.GetManagedObjectAndProperties(this.ManagedObject, "resourcePool", "ResourcePool", new string[1] { "name" });
            IVimResourcePool vimResourcePool = (IVimResourcePool)null;
            foreach (ManagedObjectAndProperties objectAndProperties2 in objectAndProperties1)
            {
                if (!(objectAndProperties2.ManagedObject.type != "ResourcePool") && objectAndProperties2.Properties != null)
                {
                    vimResourcePool = (IVimResourcePool)new ResourcePool(this.VcService, objectAndProperties2.ManagedObject);
                    vimResourcePool.Name = (string)objectAndProperties2.Properties["name"];
                    break;
                }
            }
            return vimResourcePool;
        }

        public VirtualMachineConfigSpec GetCompatibleConfigSpec(string targetDatastore, string replicaDisplayName)
        {
            Dictionary<string, object> properties = this.GetProperties(new string[1] { "config" });
            if (!properties.ContainsKey("config"))
                return (VirtualMachineConfigSpec)null;
            return VmHelper.CreateVmConfigSpec((VirtualMachineConfigInfo)properties["config"], targetDatastore, replicaDisplayName);
        }

        private class VMWareFilenameEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                string strA1 = x.Substring(0, x.IndexOf(']') + 1);
                string strB1 = y.Substring(0, y.IndexOf(']') + 1);
                string strA2 = x.Substring(strA1.Length).TrimStart((char[])null);
                string strB2 = y.Substring(strB1.Length).TrimStart((char[])null);
                return string.Compare(strA1, strB1, true) == 0 && string.Compare(strA2, strB2, true) == 0;
            }

            public int GetHashCode(string obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
