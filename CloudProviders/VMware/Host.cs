using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
    internal class Host : VCManagedItem, IVimHost, IVimManagedItem
    {
        public static string[] VCProperties = new string[15] { "name", "hardware.systemInfo.uuid", "summary.config.product.version", "summary.config.product.build", "hardware.systemInfo.vendor", "hardware.systemInfo.model", "summary.hardware.numNics", "summary.hardware.numCpuPkgs", "summary.hardware.numCpuCores", "summary.hardware.numCpuThreads", "summary.hardware.cpuMhz", "summary.hardware.cpuModel", "summary.quickStats.overallCpuUsage", "summary.quickStats.overallMemoryUsage", "summary.hardware.memorySize" };
        private ServerProperties _properties;
        private IVimService _vimService;

        public ServerProperties Properties
        {
            get
            {
                return this._properties;
            }
            set
            {
                this._properties = value;
            }
        }

        internal Host(IVimService vimService, ManagedObjectReference managedObject)
          : base(vimService, managedObject)
        {
            this._vimService = vimService;
        }

        internal Host(IVimService vimService, string name)
          : base(vimService, (ManagedObjectReference)null)
        {
            this.Name = name;
        }

        public string GetUuid()
        {
            return (string)this.GetProperty("hardware.systemInfo.uuid");
        }

        public ServerProperties GetCommonProperties()
        {
            if (string.IsNullOrEmpty(this._properties.Name))
                this.GetCommonProperties(this.GetProperties(Host.VCProperties));
            return this._properties;
        }

        public ServerProperties GetCommonProperties(ManagedObjectAndProperties[] managedObjects)
        {
            foreach (ManagedObjectAndProperties managedObject in managedObjects)
            {
                if (managedObject.ManagedObject.Value == this.ManagedObject.Value)
                {
                    this.GetCommonProperties(managedObject.Properties);
                    break;
                }
            }
            return this._properties;
        }

        public void GetCommonProperties(Dictionary<string, object> hostProperties)
        {
            for (int index = 0; index < 3 && (hostProperties == null || hostProperties.Count != Host.VCProperties.Length); ++index)
            {
                Thread.Sleep(1000);
                hostProperties = this.GetProperties(Host.VCProperties);
            }
            if (hostProperties.ContainsKey("name"))
                this._properties.Name = (string)hostProperties["name"];
            if (hostProperties.ContainsKey("hardware.systemInfo.uuid"))
                this._properties.Uuid = (string)hostProperties["hardware.systemInfo.uuid"];
            if (hostProperties.ContainsKey("summary.config.product.version"))
                this._properties.Version = (string)hostProperties["summary.config.product.version"];
            if (hostProperties.ContainsKey("summary.config.product.build"))
                this._properties.BuildNum = (string)hostProperties["summary.config.product.build"];
            if (hostProperties.ContainsKey("hardware.systemInfo.vendor"))
                this._properties.Vendor = (string)hostProperties["hardware.systemInfo.vendor"];
            if (hostProperties.ContainsKey("hardware.systemInfo.model"))
                this._properties.Model = (string)hostProperties["hardware.systemInfo.model"];
            if (hostProperties.ContainsKey("summary.hardware.numCpuCores"))
                this._properties.NumOfCpu = (short)hostProperties["summary.hardware.numCpuCores"];
            if (hostProperties.ContainsKey("summary.hardware.numCpuPkgs"))
                this._properties.NumOfCpuPkgs = (short)hostProperties["summary.hardware.numCpuPkgs"];
            if (hostProperties.ContainsKey("summary.hardware.numCpuThreads"))
                this._properties.NumOfCpuThreads = (short)hostProperties["summary.hardware.numCpuThreads"];
            if (hostProperties.ContainsKey("summary.hardware.cpuMhz"))
                this._properties.CpuMHz = (int)hostProperties["summary.hardware.cpuMhz"];
            if (hostProperties.ContainsKey("summary.hardware.cpuModel"))
                this._properties.ProcessorType = (string)hostProperties["summary.hardware.cpuModel"];
            if (hostProperties.ContainsKey("summary.hardware.numNics"))
                this._properties.NumOfNics = (int)hostProperties["summary.hardware.numNics"];
            if (hostProperties.ContainsKey("summary.quickStats.overallCpuUsage"))
                this._properties.CpuUsageMHz = (int)hostProperties["summary.quickStats.overallCpuUsage"];
            if (hostProperties.ContainsKey("summary.quickStats.overallMemoryUsage"))
                this._properties.MemoryUsageMB = (long)(int)hostProperties["summary.quickStats.overallMemoryUsage"];
            if (hostProperties.ContainsKey("summary.hardware.memorySize"))
                this._properties.MemoryMB = (long)hostProperties["summary.hardware.memorySize"] / 1048576L;
            this.Name = this._properties.Name;
        }

        public override IVimManagedItem[] GetChildren()
        {
            return (IVimManagedItem[])null;
        }

        public IVimVm[] GetVmsAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties)
        {
            List<IVimVm> vimVmList = new List<IVimVm>();
            foreach (ManagedObjectAndProperties objectsAndProperty in managedObjectsAndProperties)
            {
                if (!(objectsAndProperty.ManagedObject.type != "VirtualMachine"))
                {
                    IVimVm vimVm = (IVimVm)new Vm(this.VcService, objectsAndProperty.ManagedObject);
                    vimVm.GetCommonProperties(objectsAndProperty.Properties);
                    if (!vimVm.VMProperties.IsTemplate)
                        vimVmList.Add(vimVm);
                }
            }
            return vimVmList.ToArray();
        }

        public IVimVm[] GetVmsAndProperties()
        {
            return this.GetVmsAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "vm", "VirtualMachine", Vm.VCProperties));
        }

        public IVimDatastore[] GetDatastoresAndProperties()
        {
            return this.GetDatastoresAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "datastore", "Datastore", Datastore.VCProperties));
        }

        public IVimDatastore[] GetDatastoresAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties)
        {
            List<Datastore> datastoreList = new List<Datastore>();
            foreach (ManagedObjectAndProperties objectsAndProperty in managedObjectsAndProperties)
            {
                if (!(objectsAndProperty.ManagedObject.type != "Datastore") && objectsAndProperty.Properties != null)
                {
                    Datastore datastore = new Datastore(this.VcService, objectsAndProperty.ManagedObject);
                    datastore.GetCommonProperties(objectsAndProperty.Properties);
                    if (!string.IsNullOrEmpty(datastore.DsProperties.RemoteId))
                        datastoreList.Add(datastore);
                }
            }
            return (IVimDatastore[])datastoreList.ToArray();
        }

        public IVimNetwork[] GetNetworks()
        {
            return this.GetNetworksAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "network", "Network", Network.VCProperties));
        }

        public IVimNetwork[] GetNetworksAndProperties(ManagedObjectAndProperties[] managedObjectsAndProperties)
        {
            List<Network> networkList = new List<Network>();
            Dictionary<string, string> virtualPortgroups = this.GetDistributedVirtualPortgroups();
            foreach (ManagedObjectAndProperties objectsAndProperty in managedObjectsAndProperties)
            {
                if ((!(objectsAndProperty.ManagedObject.type != "Network") || !(objectsAndProperty.ManagedObject.type != "DistributedVirtualPortgroup")) && objectsAndProperty.Properties != null)
                {
                    Network network = new Network(this.VcService, objectsAndProperty.ManagedObject);
                    network.GetCommonProperties(objectsAndProperty.Properties);
                    if (!network.IsDistributed || virtualPortgroups.ContainsKey(network.PortgroupKey))
                        networkList.Add(network);
                }
            }
            return (IVimNetwork[])networkList.ToArray();
        }

        public Dictionary<string, string> GetDistributedVirtualPortgroups()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            try
            {
                DVSManagerDvsConfigTarget managerDvsConfigTarget = this._vimService.Service.QueryDvsConfigTarget(((VCService)this._vimService).DVSManager, this.ManagedObject, (ManagedObjectReference)null);
                if (managerDvsConfigTarget.distributedVirtualPortgroup != null)
                {
                    foreach (DistributedVirtualPortgroupInfo virtualPortgroupInfo in managerDvsConfigTarget.distributedVirtualPortgroup)
                    {
                        if (!virtualPortgroupInfo.uplinkPortgroup)
                            CUtils.AddOrReplace<string, string>((IDictionary<string, string>)dictionary, virtualPortgroupInfo.portgroupKey, virtualPortgroupInfo.portgroupName + " (" + virtualPortgroupInfo.switchName + ")");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dictionary;
        }

        public Dictionary<string, string> GetDistributedVirtualSwitchUuids()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            try
            {
                DVSManagerDvsConfigTarget managerDvsConfigTarget = this._vimService.Service.QueryDvsConfigTarget(((VCService)this._vimService).DVSManager, this.ManagedObject, (ManagedObjectReference)null);
                if (managerDvsConfigTarget.distributedVirtualPortgroup != null)
                {
                    foreach (DistributedVirtualPortgroupInfo virtualPortgroupInfo in managerDvsConfigTarget.distributedVirtualPortgroup)
                    {
                        if (!virtualPortgroupInfo.uplinkPortgroup)
                            CUtils.AddOrReplace<string, string>((IDictionary<string, string>)dictionary, virtualPortgroupInfo.portgroupKey, virtualPortgroupInfo.switchUuid);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dictionary;
        }

        public IVimHost[] GetHosts()
        {
            SelectionSpec selectionSpec = new SelectionSpec() { name = "recurseFolders" };
            TraversalSpec traversalSpec1 = new TraversalSpec() { type = "Datacenter", path = "hostFolder", skip = true, selectSet = new SelectionSpec[1] { selectionSpec } };
            TraversalSpec traversalSpec2 = new TraversalSpec();
            traversalSpec2.type = "Folder";
            traversalSpec2.name = "recurseFolders";
            traversalSpec2.path = "childEntity";
            traversalSpec2.skip = false;
            traversalSpec2.selectSet = new SelectionSpec[2]
            {
        selectionSpec,
        (SelectionSpec) traversalSpec1
            };
            TraversalSpec traversalSpec3 = traversalSpec2;
            IVimService vcService = this.VcService;
            PropertyFilterSpec[] pfSpec = new PropertyFilterSpec[1];
            int index1 = 0;
            PropertyFilterSpec propertyFilterSpec1 = new PropertyFilterSpec();
            propertyFilterSpec1.propSet = new PropertySpec[1]
            {
        new PropertySpec()
        {
          all = false,
          type = "Folder",
          pathSet = new string[2]
          {
            "name",
            "childEntity"
          }
        }
            };
            propertyFilterSpec1.objectSet = new ObjectSpec[1]
            {
        new ObjectSpec()
        {
          obj = ((VCService) this.VcService).Root,
          skip = true,
          selectSet = (SelectionSpec[]) new TraversalSpec[1]
          {
            traversalSpec3
          }
        }
            };
            PropertyFilterSpec propertyFilterSpec2 = propertyFilterSpec1;
            pfSpec[index1] = propertyFilterSpec2;
            ObjectContent[] objectContentArray = vcService.RetrieveProperties(pfSpec);
            Host[] hostArray = (Host[])null;
            for (int index2 = 0; index2 < objectContentArray.Length; ++index2)
            {
                ManagedObjectReference managedObjectReference = (ManagedObjectReference)((Array)objectContentArray[index2].propSet[0].val).GetValue(0);
                if (managedObjectReference.type == "ComputeResource" || managedObjectReference.type == "ClusterComputeResource")
                {
                    ManagedObjectReference[] managedObjects = this.GetManagedObjects(new string[1] { "host" });
                    if (managedObjects.Length != 0)
                    {
                        hostArray = new Host[managedObjects.Length];
                        for (int index3 = 0; index3 < managedObjects.Length; ++index3)
                        {
                            ManagedObjectReference managedObject = managedObjects[index3];
                            if (managedObject != null)
                            {
                                hostArray[index3] = new Host(this.VcService, managedObject);
                                Dictionary<string, object> properties = hostArray[index3].GetProperties(new string[3] { "name", "summary.config.name", "summary.config.product.version" });
                                hostArray[index3].Name = (string)properties["name"];
                            }
                        }
                    }
                }
            }
            return (IVimHost[])hostArray;
        }

        public IVimDatacenter GetDatacenterAndProperties()
        {
            IVimDatacenter vimDatacenter = (IVimDatacenter)null;
            TraversalSpec traversalSpec1 = new TraversalSpec();
            traversalSpec1.name = "folderTraversalSpec";
            traversalSpec1.type = "Folder";
            traversalSpec1.path = "parent";
            traversalSpec1.skip = true;
            traversalSpec1.selectSet = new SelectionSpec[1]
            {
        new SelectionSpec()
            };
            traversalSpec1.selectSet[0].name = "folderTraversalSpec";
            TraversalSpec traversalSpec2 = new TraversalSpec();
            traversalSpec2.name = "computeResourceTraversalSpec";
            traversalSpec2.type = "ComputeResource";
            traversalSpec2.path = "parent";
            traversalSpec2.skip = true;
            traversalSpec2.selectSet = new SelectionSpec[2]
            {
        (SelectionSpec) traversalSpec1,
        new SelectionSpec()
            };
            traversalSpec2.selectSet[1].name = "computeResourceTraversalSpec";
            TraversalSpec traversalSpec3 = new TraversalSpec();
            traversalSpec3.name = "hostTraversalSpec";
            traversalSpec3.type = "HostSystem";
            traversalSpec3.path = "parent";
            traversalSpec3.skip = true;
            traversalSpec3.selectSet = new SelectionSpec[1]
            {
        (SelectionSpec) traversalSpec2
            };
            PropertySpec[] propertySpecArray = new PropertySpec[1] { new PropertySpec() };
            propertySpecArray[0].all = false;
            propertySpecArray[0].pathSet = Datacenter.VCProperties;
            propertySpecArray[0].type = "Datacenter";
            PropertyFilterSpec propertyFilterSpec = new PropertyFilterSpec();
            propertyFilterSpec.propSet = propertySpecArray;
            propertyFilterSpec.objectSet = new ObjectSpec[1]
            {
        new ObjectSpec()
            };
            propertyFilterSpec.objectSet[0].obj = this.ManagedObject;
            propertyFilterSpec.objectSet[0].skip = false;
            propertyFilterSpec.objectSet[0].selectSet = new SelectionSpec[1]
            {
        (SelectionSpec) traversalSpec3
            };
            IVimService vcService = this.VcService;
            PropertyFilterSpec[] pfSpec = new PropertyFilterSpec[1] { propertyFilterSpec };
            foreach (ObjectContent retrieveProperty in vcService.RetrieveProperties(pfSpec))
            {
                vimDatacenter = (IVimDatacenter)new Datacenter(this.VcService, retrieveProperty.obj);
                Dictionary<string, object> dictionary = this.VcService.PropSetToDictionary(retrieveProperty.propSet);
                vimDatacenter.GetCommonProperties(dictionary);
            }
            return vimDatacenter;
        }

        public IVimDatastore GetDatastoreByUrl(string url)
        {
            IVimDatastore[] datastoresAndProperties = this.GetDatastoresAndProperties();
            IVimDatastore vimDatastore1 = (IVimDatastore)null;
            foreach (IVimDatastore vimDatastore2 in datastoresAndProperties)
            {
                if (string.Compare(url, vimDatastore2.DsProperties.Url, true, CultureInfo.InvariantCulture) == 0)
                {
                    vimDatastore1 = vimDatastore2;
                    break;
                }
            }
            return vimDatastore1;
        }

        public override string GetName()
        {
            this.Name = (string)this.GetProperty("name");
            return this.Name;
        }

        public IVimDatastore GetDatastoreByName(string name)
        {
            IVimDatastore[] datastoresAndProperties = this.GetDatastoresAndProperties();
            IVimDatastore vimDatastore1 = (IVimDatastore)null;
            foreach (IVimDatastore vimDatastore2 in datastoresAndProperties)
            {
                if (string.Compare(name, vimDatastore2.DsProperties.Name, true, CultureInfo.InvariantCulture) == 0)
                {
                    vimDatastore1 = vimDatastore2;
                    break;
                }
            }
            return vimDatastore1;
        }

        public long GetDatastoreMaxVmdkSizeMB(string url)
        {
            return this.GetDatastoreMaxVmdkSizeMB(this.GetDatastoreByUrl(url));
        }

        public long GetDatastoreMaxVmdkSizeMB(IVimDatastore ds)
        {
            long num = 0;
            if (ds != null)
            {
                num = ds.DsProperties.MaxFileSize / 1048576L;
                if (ds.DsProperties.Type.Equals("VMFS") && ds.DsProperties.Version.StartsWith("5"))
                    num = this._properties.Version.StartsWith("5.0") || this._properties.Version.StartsWith("5.1") ? 2097151L : 65011712L;
            }
            if (num == 0L || num == 2097152L)
                num = 2097151L;
            return num;
        }

        public IVimVm SearchVmByUuid(string uuid)
        {
            return this.VcService.SearchVmByUuid(uuid);
        }

        public string GetDatastorePathByUrl(string url)
        {
            IVimDatastore datastoreByUrl = this.GetDatastoreByUrl(url);
            if (datastoreByUrl == null)
                return (string)null;
            return datastoreByUrl.GetPath();
        }

        public void UnregisterVm(IVimVm vm)
        {
            this.VcService.UnregisterVm(vm);
        }

        public IVimVm RegisterVm(string dsPath, string resPoolName, VimClientlContext ctx)
        {
            ManagedObjectReference vmFolder = this.GetDatacenterAndProperties().DatacenterProperties.VmFolder;
            if (vmFolder == null)
                throw new EsxException("vmFolder is null", false);
            IVimResourcePool vimResourcePool = (IVimResourcePool)null;
            if (!string.IsNullOrEmpty(resPoolName))
                vimResourcePool = this.GetResourcePoolByName(resPoolName);
            if (vimResourcePool == null)
                vimResourcePool = this.GetDefaultResourcePool();
            ManagedObjectReference managedObject = this.VcService.Service.RegisterVM_Task(vmFolder, dsPath, (string)null, false, vimResourcePool.ManagedObject, this.ManagedObject);
            ctx.IsRetriableCall = false;
            Task task = new Task(this.VcService, managedObject);
            string op = "RegisterVm";
            VimClientlContext rstate = ctx;
            task.WaitForResult(op, rstate);
            string[] properties1 = new string[1] { "info.result" };
            Dictionary<string, object> properties2 = task.GetProperties(properties1);
            if (properties2.ContainsKey("info.result"))
                return (IVimVm)new Vm(this.VcService, (ManagedObjectReference)properties2["info.result"]);
            throw new EsxException("Vm managed object reference does not exist", true);
        }

        public ManagedObjectReference GetComputeResource()
        {
            return this.GetManagedObjects(new string[1] { "parent" })[0];
        }

        public IVimResourcePool[] GetAllResourcePools()
        {
            ManagedObjectReference computeResource = this.GetComputeResource();
            TraversalSpec traversalSpec1 = new TraversalSpec();
            traversalSpec1.name = "resourcePoolTraversalSpec";
            traversalSpec1.type = "ResourcePool";
            traversalSpec1.path = "resourcePool";
            traversalSpec1.skip = true;
            traversalSpec1.selectSet = new SelectionSpec[1]
            {
        new SelectionSpec()
            };
            traversalSpec1.selectSet[0].name = "resourcePoolTraversalSpec";
            TraversalSpec traversalSpec2 = new TraversalSpec();
            traversalSpec2.name = "computeResourceTraversalSpec";
            traversalSpec2.type = "ComputeResource";
            traversalSpec2.path = "resourcePool";
            traversalSpec2.skip = true;
            traversalSpec2.selectSet = new SelectionSpec[1]
            {
        (SelectionSpec) traversalSpec1
            };
            PropertySpec[] propertySpecArray = new PropertySpec[1] { new PropertySpec() };
            propertySpecArray[0].all = false;
            propertySpecArray[0].pathSet = new string[2]
            {
        "name",
        "parent"
            };
            propertySpecArray[0].type = "ResourcePool";
            PropertyFilterSpec propertyFilterSpec = new PropertyFilterSpec();
            propertyFilterSpec.propSet = propertySpecArray;
            propertyFilterSpec.objectSet = new ObjectSpec[1]
            {
        new ObjectSpec()
            };
            propertyFilterSpec.objectSet[0].obj = computeResource;
            propertyFilterSpec.objectSet[0].skip = false;
            propertyFilterSpec.objectSet[0].selectSet = new SelectionSpec[1]
            {
        (SelectionSpec) traversalSpec2
            };
            ObjectContent[] objectContentArray = this.VcService.RetrieveProperties(new PropertyFilterSpec[1] { propertyFilterSpec });
            List<ResourcePool> resourcePoolList = new List<ResourcePool>();
            foreach (ObjectContent objectContent in objectContentArray)
            {
                ResourcePool resourcePool = new ResourcePool(this.VcService, objectContent.obj);
                Dictionary<string, object> dictionary = this.VcService.PropSetToDictionary(objectContent.propSet);
                resourcePool.GetCommonProperties(dictionary);
                resourcePoolList.Add(resourcePool);
            }
            return (IVimResourcePool[])resourcePoolList.ToArray();
        }

        public IVimResourcePool GetDefaultResourcePool()
        {
            ManagedObjectAndProperties[] objectAndProperties1 = this.GetManagedObjectAndProperties(this.GetComputeResource(), "resourcePool", "ResourcePool", new string[2] { "name", "parent" });
            ResourcePool resourcePool = (ResourcePool)null;
            foreach (ManagedObjectAndProperties objectAndProperties2 in objectAndProperties1)
            {
                if (!(objectAndProperties2.ManagedObject.type != "ResourcePool") && objectAndProperties2.Properties != null)
                {
                    resourcePool = new ResourcePool(this.VcService, objectAndProperties2.ManagedObject);
                    resourcePool.Name = (string)objectAndProperties2.Properties["name"];
                    resourcePool.Parent = (ManagedObjectReference)objectAndProperties2.Properties["parent"];
                    break;
                }
            }
            return (IVimResourcePool)resourcePool;
        }

        public IVimResourcePool GetResourcePoolByName(string resPoolName)
        {
            IVimResourcePool vimResourcePool = (IVimResourcePool)null;
            if (!string.IsNullOrEmpty(resPoolName))
            {
                foreach (IVimResourcePool allResourcePool in this.GetAllResourcePools())
                {
                    if (string.Compare(allResourcePool.Name, resPoolName, true, CultureInfo.InvariantCulture) == 0)
                    {
                        vimResourcePool = allResourcePool;
                        break;
                    }
                }
            }
            return vimResourcePool;
        }

        public void MoveVmToResourcePool(IVimVm vm, string resPoolName)
        {
            IVimResourcePool[] allResourcePools = this.GetAllResourcePools();
            IVimResourcePool vimResourcePool1 = (IVimResourcePool)null;
            foreach (IVimResourcePool vimResourcePool2 in allResourcePools)
            {
                if (string.Compare(vimResourcePool2.Name, resPoolName, true, CultureInfo.InvariantCulture) == 0)
                {
                    vimResourcePool1 = vimResourcePool2;
                    break;
                }
            }
            if (vimResourcePool1 == null)
                return;
            this.VcService.Service.MoveIntoResourcePool(vimResourcePool1.ManagedObject, new ManagedObjectReference[1]
            {
        vm.ManagedObject
            });
        }

        public string ContainsVmName(string vmName)
        {
            string str = (string)null;
            foreach (IVimVm vmsAndProperty in this.GetVmsAndProperties(this.GetManagedObjectAndProperties(this.ManagedObject, "vm", "VirtualMachine", Vm.VCProperties)))
            {
                if (string.Compare(vmsAndProperty.Name, vmName, true, CultureInfo.InvariantCulture) == 0)
                {
                    str = vmsAndProperty.VMProperties.Uuid;
                    break;
                }
            }
            return str;
        }

        public short GetNumberCPU()
        {
            return (short)this.GetProperties(new string[1] { "summary.hardware.numCpuCores" })["summary.hardware.numCpuCores"];
        }

        public short GetNumberCpuThreads()
        {
            return (short)this.GetProperties(new string[1] { "summary.hardware.numCpuThreads" })["summary.hardware.numCpuThreads"];
        }

        public short GetNumberCpuPackages()
        {
            return (short)this.GetProperties(new string[1] { "summary.hardware.numCpuPkgs" })["summary.hardware.numCpuPkgs"];
        }

        public long GetMemory()
        {
            return (long)this.GetProperties(new string[1] { "summary.hardware.memorySize" })["summary.hardware.memorySize"];
        }

        public HostConfiguration GetConfiguration()
        {
            return new HostConfiguration() { NumCPU = this.GetNumberCPU(), NumCpuPkgs = this.GetNumberCpuPackages(), NumCpuThreads = this.GetNumberCpuThreads(), Memory = this.GetMemory(), Networks = this.GetNetworks() };
        }

        public Dictionary<string, string[]> GetVirtualSwitch()
        {
            HostVirtualSwitch[] property1 = (HostVirtualSwitch[])this.GetProperties(new string[1] { "config.network.vswitch" })["config.network.vswitch"];
            PhysicalNic[] property2 = (PhysicalNic[])this.GetProperties(new string[1] { "config.network.pnic" })["config.network.pnic"];
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            List<string> stringList = new List<string>();
            foreach (HostVirtualSwitch hostVirtualSwitch in property1)
            {
                PhysicalNic[] property3 = (PhysicalNic[])this.GetProperties(new string[1] { "config.network.vswitch[ \" " + hostVirtualSwitch.key + " \"].pnic" })["config.network.vswitch[" + hostVirtualSwitch.key + "].pnic"];
                string[] pnic = hostVirtualSwitch.pnic;
                if (pnic != null)
                {
                    foreach (PhysicalNic physicalNic in property2)
                    {
                        foreach (string str in pnic)
                        {
                            if (physicalNic.key == str)
                            {
                                stringList.Add(str);
                                break;
                            }
                        }
                    }
                    string[] array = stringList.ToArray();
                    if (!dictionary.ContainsKey(hostVirtualSwitch.name))
                        dictionary.Add(hostVirtualSwitch.name, array);
                }
                else if (!dictionary.ContainsKey(hostVirtualSwitch.name))
                    dictionary.Add(hostVirtualSwitch.name, new string[1]
                    {
            "No pNic is attached."
                    });
            }
            return dictionary;
        }

        public IVimVm GetVm(string name)
        {
            IVimVm vimVm = (IVimVm)null;
            foreach (IVimVm vmsAndProperty in this.GetVmsAndProperties())
            {
                if (string.Compare(vmsAndProperty.VMProperties.Name, name, true, CultureInfo.InvariantCulture) == 0)
                {
                    vimVm = vmsAndProperty;
                    break;
                }
            }
            return vimVm;
        }

        public IVimVm GetVmByUuid(string uuid)
        {
            IVimVm vimVm = (IVimVm)null;
            foreach (IVimVm vmsAndProperty in this.GetVmsAndProperties())
            {
                if (string.Compare(vmsAndProperty.VMProperties.Uuid, uuid, true, CultureInfo.InvariantCulture) == 0)
                {
                    vimVm = vmsAndProperty;
                    break;
                }
            }
            return vimVm;
        }

        public IVimVm CreateVm(VmCreationInfo vmCreationInfo, VimClientlContext ctx)
        {
            VirtualMachineConfigSpec vmConfigSpec = this.createVmConfigSpec(vmCreationInfo);
            vmConfigSpec.guestId = vmCreationInfo.GuestId;
            vmConfigSpec.memoryMB = vmCreationInfo.MemoryMB;
            vmConfigSpec.memoryMBSpecified = true;
            vmConfigSpec.name = vmCreationInfo.Name;
            vmConfigSpec.numCPUs = vmCreationInfo.NumCPU;
            vmConfigSpec.numCPUsSpecified = true;
            vmConfigSpec.numCoresPerSocket = Math.Max(1, vmCreationInfo.NumCoresPerProcessor);
            vmConfigSpec.numCoresPerSocketSpecified = true;
            return this.CreateVm(vmConfigSpec, ctx);
        }
        public IVimVm CreateVm(VmCreationInfo vmCreationInfo, VimClientlContext ctx, string template)
        {
            VirtualMachineCloneSpec virtualMachineCloneSpec = new VirtualMachineCloneSpec();
            //virtualMachineCloneSpec.location = new VirtualMachineRelocateSpec();
            //virtualMachineCloneSpec.location.datastore = chosenDataStore;
            //virtualMachineCloneSpec.location.pool = resourcePool.MoRef;
            //virtualMachineCloneSpec.powerOn = false;
            //virtualMachineCloneSpec.template = false;
            //virtualMachineCloneSpec.customization = new CustomizationSpec()
            //{
            //    globalIPSettings = new CustomizationGlobalIPSettings()
            //    {
            //        dnsServerList = [""],
            //        dnsSuffixList = [""]
            //    },
            //    identity = new CustomizationIdentitySettings()
            //    {

            //    },
            //    nicSettingMap = [new CustomizationAdapterMapping()
            //    {
            //        macAddress = "dynamic",
            //        adapter = new CustomizationIPSettings()
            //        {
            //            dnsDomain = ""
            //        }
            //    }],
            //    options = new CustomizationOptions()
            //    {

            //    }
            //};

            //// Get Template that needs to be deployed to a VM
            //NameValueCollection filterForBaseTemplate = new NameValueCollection();
            //filterForBaseTemplate.Add("Name", "Win2003R2SP2ESX01");
            //VirtualMachine baseTemplate = (VirtualMachine)vimClient.FindEntityView(typeof(VirtualMachine), null, filterForBaseTemplate, null);


            //VirtualMachineConfigSpec vmConfigSpec = this.createVmConfigSpec(vmCreationInfo);
            //vmConfigSpec.guestId = vmCreationInfo.GuestId;
            //vmConfigSpec.memoryMB = vmCreationInfo.MemoryMB;
            //vmConfigSpec.memoryMBSpecified = true;
            //vmConfigSpec.name = vmCreationInfo.Name;
            //vmConfigSpec.numCPUs = vmCreationInfo.NumCPU;
            //vmConfigSpec.numCPUsSpecified = true;
            //vmConfigSpec.numCoresPerSocket = Math.Max(1, vmCreationInfo.NumCoresPerProcessor);
            //vmConfigSpec.numCoresPerSocketSpecified = true;
            //return this.CreateVm(vmConfigSpec, ctx);
        }
        // Set up Clone Spec.
        public IVimVm CreateVmWithNetworkMapping(VirtualMachineConfigSpec configSpec, Dictionary<string, string> networkMap, VimClientlContext ctx)
        {
            Dictionary<string, IVimNetwork> networksDict = new Dictionary<string, IVimNetwork>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            ((IEnumerable<IVimNetwork>)this.GetNetworks()).ForEach<IVimNetwork>((System.Action<IVimNetwork>)(network =>
          {
              if (networksDict.ContainsKey(network.Name))
                  return;
              networksDict.Add(network.Name, network);
          }));
            Dictionary<string, string> dvPortgroupUuids = this.GetDistributedVirtualSwitchUuids();
            ((IEnumerable<VirtualDeviceConfigSpec>)configSpec.deviceChange).Where<VirtualDeviceConfigSpec>((Func<VirtualDeviceConfigSpec, bool>)(vdcs => vdcs.device is VirtualEthernetCard)).ForEach<VirtualDeviceConfigSpec>((System.Action<VirtualDeviceConfigSpec>)(vdcs =>
         {
             string key;
             IVimNetwork vimNetwork;
             if (!networkMap.TryGetValue(vdcs.device.deviceInfo.summary, out key) || !networksDict.TryGetValue(key, out vimNetwork))
                 throw new EsxException(string.Format("Don't know how to map the network connection \"{0}\"", (object)vdcs.device.deviceInfo.summary), false);
             if (vimNetwork.IsDistributed)
             {
                 string str;
                 if (!dvPortgroupUuids.TryGetValue(vimNetwork.PortgroupKey, out str))
                     return;
                 vdcs.device.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardDistributedVirtualPortBackingInfo();
                 ((VirtualEthernetCardDistributedVirtualPortBackingInfo)vdcs.device.backing).port = new DistributedVirtualSwitchPortConnection()
                 {
                     switchUuid = str,
                     portgroupKey = vimNetwork.PortgroupKey
                 };
             }
             else
             {
                 vdcs.device.backing = (VirtualDeviceBackingInfo)new VirtualEthernetCardNetworkBackingInfo();
                 ((VirtualEthernetCardNetworkBackingInfo)vdcs.device.backing).network = networksDict[key].ManagedObject;
                 ((VirtualDeviceDeviceBackingInfo)vdcs.device.backing).deviceName = networksDict[key].Name;
             }
         }));
            return this.CreateVm(configSpec, ctx);
        }

        public IVimVm CreateVm(VirtualMachineConfigSpec configSpec, VimClientlContext ctx)
        {
            ManagedObjectReference vmFolder = this.GetDatacenterAndProperties().GetVmFolder();
            IVimResourcePool defaultResourcePool = this.GetDefaultResourcePool();
            long num1 = configSpec.memoryMB % 4L;
            if (num1 != 0L)
                configSpec.memoryMB += 4L - num1;
            Task task = new Task(this.VcService, this.VcService.Service.CreateVM_Task(vmFolder, configSpec, defaultResourcePool.ManagedObject, this.ManagedObject));
            task.WaitForResult("CreateVm", ctx);
            Dictionary<string, object> dictionary = (Dictionary<string, object>)null;
            TaskInfoState taskInfoState = TaskInfoState.running;
            int num2;
            for (num2 = 0; num2 < 12; ++num2)
            {
                dictionary = task.GetProperties(new string[2]
                {
          "info.result",
          "info.state"
                });
                if (!dictionary.ContainsKey("info.result"))
                    Thread.Sleep(5000);
                else
                    break;
            }
            if (num2 == 12)
            {
                if (dictionary.ContainsKey("info.state"))
                    throw new EsxException("Vm creation timed out. Status returned by CreateVM_Task: (TaskInfoState) " + (object)(TaskInfoState)dictionary["info.state"], false);
                throw new EsxException("Vm creation timed out. ESX async task issue. Check host performance.", false);
            }
            if (!dictionary.ContainsKey("info.result"))
                throw new EsxException("Vm managed object reference does not exist", false);
            for (int index = 0; index < 60; ++index)
            {
                taskInfoState = (TaskInfoState)dictionary["info.state"];
                if (taskInfoState != TaskInfoState.success)
                    Thread.Sleep(1000);
                else
                    break;
            }
            if (taskInfoState != TaskInfoState.success)
                throw new EsxException("Task State was never set to Success", false);
            IVimVm vimVm = (IVimVm)new Vm(this.VcService, (ManagedObjectReference)dictionary["info.result"]);
            for (int index = 0; index < 60; ++index)
            {
                if (!string.IsNullOrEmpty(vimVm.Uuid))
                    return vimVm;
                Thread.Sleep(5000);
            }
            throw new EsxException("Vm creation timed out.  Check host performance.", false);
        }

        private VirtualDevice[] getDefaultDevices(ManagedObjectReference computeResource)
        {
            VirtualMachineConfigOption machineConfigOption = this.VcService.Service.QueryConfigOption(this.GetManagedObjects(computeResource, new string[1] { "environmentBrowser" })[0], (string)null, this.ManagedObject);
            VirtualDevice[] virtualDeviceArray = (VirtualDevice[])null;
            if (machineConfigOption != null)
                virtualDeviceArray = machineConfigOption.defaultDevice;
            return virtualDeviceArray;
        }

        private ConfigTarget getConfigTargetForHost(ManagedObjectReference computeResource)
        {
            return this.VcService.Service.QueryConfigTarget(this.GetManagedObjects(computeResource, new string[1] { "environmentBrowser" })[0], this.ManagedObject);
        }

        private VirtualDeviceConfigSpec createVirtualDiskConfigSpec(string vmName, VmDiskInfo diskInfo)
        {
            string str = this.GetVolumeName(diskInfo.Location.Name);
            if (!string.IsNullOrEmpty(diskInfo.File))
                str = str + vmName + "/" + diskInfo.File;
            VirtualDiskFlatVer2BackingInfo flatVer2BackingInfo = new VirtualDiskFlatVer2BackingInfo();
            flatVer2BackingInfo.datastore = diskInfo.Location.ManagedObject;
            flatVer2BackingInfo.fileName = str;
            flatVer2BackingInfo.diskMode = diskInfo.Mode;
            VirtualDisk virtualDisk = new VirtualDisk();
            virtualDisk.key = -1;
            virtualDisk.controllerKey = diskInfo.CtrlKey;
            virtualDisk.controllerKeySpecified = true;
            virtualDisk.unitNumber = diskInfo.UnitNumber;
            virtualDisk.unitNumberSpecified = true;
            virtualDisk.backing = (VirtualDeviceBackingInfo)flatVer2BackingInfo;
            virtualDisk.capacityInKB = diskInfo.SizeMB * 1024L;
            return new VirtualDeviceConfigSpec() { fileOperation = VirtualDeviceConfigSpecFileOperation.create, fileOperationSpecified = true, operation = VirtualDeviceConfigSpecOperation.add, operationSpecified = true, device = (VirtualDevice)virtualDisk };
        }

        private VirtualDeviceConfigSpec addVirtualDiskConfigSpec(VmDiskInfo diskInfo)
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
            return new VirtualDeviceConfigSpec() { operation = VirtualDeviceConfigSpecOperation.add, operationSpecified = true, device = (VirtualDevice)virtualDisk };
        }

        private VirtualMachineConfigSpec createVmConfigSpec(VmCreationInfo vmCreationInfo)
        {
            ManagedObjectReference computeResource = this.GetComputeResource();
            VirtualMachineConfigSpec machineConfigSpec = new VirtualMachineConfigSpec();
            VirtualDevice[] defaultDevices = this.getDefaultDevices(computeResource);
            this.getConfigTargetForHost(computeResource);
            string volumeName = this.GetVolumeName(vmCreationInfo.Location.Name);
            machineConfigSpec.files = new VirtualMachineFileInfo()
            {
                vmPathName = volumeName
            };
            int num = 1;
            VirtualSCSIController virtualScsiController = vmCreationInfo.ScsiControllerType != ScsiControllerType.LsiLogicSAS ? (vmCreationInfo.ScsiControllerType != ScsiControllerType.LsiLogicParallel ? (VirtualSCSIController)new VirtualBusLogicController() : (VirtualSCSIController)new VirtualLsiLogicController()) : (VirtualSCSIController)new VirtualLsiLogicSASController();
            virtualScsiController.busNumber = 0;
            virtualScsiController.key = num;
            virtualScsiController.sharedBus = VirtualSCSISharing.noSharing;
            VirtualDeviceConfigSpec deviceConfigSpec1 = new VirtualDeviceConfigSpec();
            deviceConfigSpec1.operation = VirtualDeviceConfigSpecOperation.add;
            deviceConfigSpec1.operationSpecified = true;
            deviceConfigSpec1.device = (VirtualDevice)virtualScsiController;
            List<VirtualDeviceConfigSpec> deviceConfigSpecList1 = new List<VirtualDeviceConfigSpec>();
            if (vmCreationInfo.Disks != null)
            {
                foreach (VmDiskInfo disk in vmCreationInfo.Disks)
                {
                    VirtualDeviceConfigSpec deviceConfigSpec2 = !disk.Exists ? this.createVirtualDiskConfigSpec(vmCreationInfo.Name, disk) : this.addVirtualDiskConfigSpec(disk);
                    if (deviceConfigSpec2 != null)
                        deviceConfigSpecList1.Add(deviceConfigSpec2);
                }
            }
            VirtualDevice virtualDevice = (VirtualDevice)null;
            for (int index = 0; index < defaultDevices.Length; ++index)
            {
                if (defaultDevices[index] is VirtualIDEController)
                {
                    virtualDevice = defaultDevices[index];
                    break;
                }
            }
            VirtualDeviceConfigSpec deviceConfigSpec3 = (VirtualDeviceConfigSpec)null;
            if (virtualDevice != null)
            {
                VirtualCdromRemotePassthroughBackingInfo passthroughBackingInfo = new VirtualCdromRemotePassthroughBackingInfo();
                passthroughBackingInfo.exclusive = false;
                passthroughBackingInfo.deviceName = "";
                VirtualCdrom virtualCdrom = new VirtualCdrom();
                virtualCdrom.backing = (VirtualDeviceBackingInfo)passthroughBackingInfo;
                virtualCdrom.key = -1;
                virtualCdrom.controllerKey = virtualDevice.key;
                virtualCdrom.controllerKeySpecified = true;
                virtualCdrom.unitNumber = 0;
                virtualCdrom.unitNumberSpecified = true;
                deviceConfigSpec3 = new VirtualDeviceConfigSpec();
                deviceConfigSpec3.operation = VirtualDeviceConfigSpecOperation.add;
                deviceConfigSpec3.operationSpecified = true;
                deviceConfigSpec3.device = (VirtualDevice)virtualCdrom;
            }
            IVimNetwork[] networks = this.GetNetworks();
            Dictionary<string, Network> dictionary = new Dictionary<string, Network>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
            foreach (Network network in networks)
            {
                if (!dictionary.ContainsKey(network.Name))
                    dictionary.Add(network.Name, network);
            }
            Dictionary<string, string> virtualSwitchUuids = this.GetDistributedVirtualSwitchUuids();
            List<VirtualDeviceConfigSpec> deviceConfigSpecList2 = new List<VirtualDeviceConfigSpec>();
            VirtualDeviceBackingInfo deviceBackingInfo = (VirtualDeviceBackingInfo)null;
            foreach (string key in vmCreationInfo.NICMapping)
            {
                if (!("---Discard---" == key))
                {
                    if (dictionary.ContainsKey(key))
                    {
                        Network network = dictionary[key];
                        if (network.IsDistributed)
                        {
                            if (virtualSwitchUuids.ContainsKey(network.PortgroupKey))
                            {
                                deviceBackingInfo = (VirtualDeviceBackingInfo)new VirtualEthernetCardDistributedVirtualPortBackingInfo();
                                ((VirtualEthernetCardDistributedVirtualPortBackingInfo)deviceBackingInfo).port = new DistributedVirtualSwitchPortConnection()
                                {
                                    switchUuid = virtualSwitchUuids[network.PortgroupKey],
                                    portgroupKey = network.PortgroupKey
                                };
                            }
                        }
                        else
                        {
                            deviceBackingInfo = (VirtualDeviceBackingInfo)new VirtualEthernetCardNetworkBackingInfo();
                            ((VirtualEthernetCardNetworkBackingInfo)deviceBackingInfo).network = dictionary[key].ManagedObject;
                            ((VirtualDeviceDeviceBackingInfo)deviceBackingInfo).deviceName = dictionary[key].Name;
                        }
                    }
                    else
                        deviceBackingInfo = (VirtualDeviceBackingInfo)new VirtualEthernetCardNetworkBackingInfo();
                    VirtualEthernetCard virtualEthernetCard = vmCreationInfo.NicType != VirtualNicType.Vmxnet3 ? (vmCreationInfo.NicType != VirtualNicType.Vmxnet ? (vmCreationInfo.NicType != VirtualNicType.E1000 ? (VirtualEthernetCard)new VirtualPCNet32() : (VirtualEthernetCard)new VirtualE1000()) : (VirtualEthernetCard)new VirtualVmxnet()) : (VirtualEthernetCard)new VirtualVmxnet3();
                    virtualEthernetCard.addressType = "generated";
                    virtualEthernetCard.backing = deviceBackingInfo;
                    virtualEthernetCard.key = -1;
                    deviceConfigSpecList2.Add(new VirtualDeviceConfigSpec()
                    {
                        operation = VirtualDeviceConfigSpecOperation.add,
                        operationSpecified = true,
                        device = (VirtualDevice)virtualEthernetCard
                    });
                }
            }
            List<VirtualDeviceConfigSpec> deviceConfigSpecList3 = new List<VirtualDeviceConfigSpec>();
            deviceConfigSpecList3.Add(deviceConfigSpec1);
            foreach (VirtualDeviceConfigSpec deviceConfigSpec2 in deviceConfigSpecList1)
                deviceConfigSpecList3.Add(deviceConfigSpec2);
            if (virtualDevice != null)
                deviceConfigSpecList3.Add(deviceConfigSpec3);
            foreach (VirtualDeviceConfigSpec deviceConfigSpec2 in deviceConfigSpecList2)
                deviceConfigSpecList3.Add(deviceConfigSpec2);
            machineConfigSpec.deviceChange = deviceConfigSpecList3.ToArray();
            return machineConfigSpec;
        }

        public IVimVm[] GetVmTemplatesAndProperties()
        {
            ManagedObjectAndProperties[] objectAndProperties1 = this.GetManagedObjectAndProperties(this.ManagedObject, "vm", "VirtualMachine", Vm.VCProperties);
            List<IVimVm> vimVmList = new List<IVimVm>();
            foreach (ManagedObjectAndProperties objectAndProperties2 in objectAndProperties1)
            {
                IVimVm vimVm = (IVimVm)new Vm(this.VcService, objectAndProperties2.ManagedObject);
                vimVm.GetCommonProperties(objectAndProperties2.Properties);
                if (vimVm.VMProperties.IsTemplate)
                    vimVmList.Add(vimVm);
            }
            return vimVmList.ToArray();
        }

        public IVimVm GetRecentlyCreatedVm(ManagedObjectReference task)
        {
            TaskInfoState taskInfoState = TaskInfoState.running;
            do
            {
                ManagedObjectReference managedObject = this.ManagedObject;
                string path = "recentTask";
                string childType = "Task";
                string[] childProperties = new string[3] { "info.completeTime", "info.descriptionId", "info.state" };
                foreach (ManagedObjectAndProperties objectAndProperty in this.GetManagedObjectAndProperties(managedObject, path, childType, childProperties))
                {
                    if (objectAndProperty.Properties != null && objectAndProperty.ManagedObject == task)
                    {
                        taskInfoState = (TaskInfoState)objectAndProperty.Properties["info.state"];
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
            while (taskInfoState == TaskInfoState.running);
            ManagedObjectReference[] managedObjects = this.GetManagedObjects(task, new string[1] { "entity" });
            IVimVm vimVm = (IVimVm)null;
            if (managedObjects.Length != 0)
                vimVm = (IVimVm)new Vm(this.VcService, managedObjects[0]);
            return vimVm;
        }

        public List<string> SearchDatastoreSubFolder(string folderName, VimClientlContext ctx)
        {
            List<string> stringList = new List<string>();
            Task task = new Task(this.VcService, this.VcService.Service.SearchDatastoreSubFolders_Task(this.GetManagedObjects(new string[1] { "datastoreBrowser" })[0], "[] " + folderName, new HostDatastoreBrowserSearchSpec() { matchPattern = new string[1] { "*.vmdk" }, searchCaseInsensitive = true, searchCaseInsensitiveSpecified = true }));
            string op = "Browse Datastore";
            VimClientlContext rstate = ctx;
            task.WaitForResult(op, rstate);
            string[] properties1 = new string[1] { "info.result" };
            Dictionary<string, object> properties2 = task.GetProperties(properties1);
            if (properties2.ContainsKey("info.result"))
            {
                HostDatastoreBrowserSearchResults[] browserSearchResultsArray = (HostDatastoreBrowserSearchResults[])properties2["info.result"];
                if (browserSearchResultsArray != null && browserSearchResultsArray.Length != 0 && browserSearchResultsArray[0].file != null)
                {
                    foreach (FileInfo fileInfo in browserSearchResultsArray[0].file)
                    {
                        if (!fileInfo.path.EndsWith("-flat.vmdk", StringComparison.InvariantCultureIgnoreCase) && !fileInfo.path.EndsWith("-delta.vmdk", StringComparison.InvariantCultureIgnoreCase) && !stringList.Contains(fileInfo.path))
                            stringList.Add(fileInfo.path);
                    }
                }
            }
            return stringList;
        }

        public bool UserHasPermissions(out int userRole)
        {
            userRole = 0;
            IVimDatacenter datacenterAndProperties = this.GetDatacenterAndProperties();
            if (datacenterAndProperties.DatacenterProperties.EffectiveRoles == null)
                return false;
            foreach (int effectiveRole in datacenterAndProperties.DatacenterProperties.EffectiveRoles)
            {
                userRole = effectiveRole;
                if (effectiveRole == -1 || effectiveRole == 1 || effectiveRole == 2)
                    return true;
            }
            return false;
        }

        public ManagedObjectAndProperties[] GetMangedObjectsAndProperties()
        {
            PropertySpec propertySpec1 = new PropertySpec();
            propertySpec1.type = "HostSystem";
            propertySpec1.all = false;
            propertySpec1.pathSet = Host.VCProperties;
            PropertySpec propertySpec2 = new PropertySpec();
            propertySpec2.type = "VirtualMachine";
            propertySpec2.all = false;
            propertySpec2.pathSet = Vm.VCProperties;
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
          type = "HostSystem",
          path = "vm"
        },
        (SelectionSpec) new TraversalSpec()
        {
          type = "HostSystem",
          path = "datastore"
        },
        (SelectionSpec) new TraversalSpec()
        {
          type = "HostSystem",
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
    }
}
