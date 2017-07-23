using System;
using System.Collections.Generic;
using System.Linq;
using VimWrapper;

using OculiService.Win32;

namespace OculiService.CloudProviders.VMware
{
    public class ConnectAllVirtualDisksESX : ConnectVirtualDiskESXCommon, ITaskCommandCommon, ITaskCommandBase
    {
        protected List<VmDiskInfo> _VmDiskInfos = new List<VmDiskInfo>();
        protected Dictionary<int, string> _VirtualDiskInfo;

        public ConnectAllVirtualDisksESX(TaskContext context)
          : base(context)
        {
        }

        public void Invoke()
        {
            this._Context.SetLowLevelState("ConfiguringAppliance");
            this._Logger.Information("Connecting all of the replica's virtual disks to the helper appliance.");
            this._GetInformation();
            this._VerifyConfiguration();
            lock (Win32Utils.HelperOSLock)
            {
                this._PrepareToConnectDrives();
                this._ConnectDrives();
                this._OnlineAllDisks();
            }
            this._Logger.Information("The replica's virtual disks are now connected to the helper appliance.");
        }

        private void _PrepareToConnectDrives()
        {
            this._Logger.Verbose("Preparing to connect drives");
            this._AvailableSCSISlots = this._GetAvailableSCSISlots();
            foreach (OculiServiceVolumePersistedState volume in this._Context.JobInfoWrapper.VolumePersistedState)
            {
                this._SetSCSISlot(volume);
                volume.VirtualDiskFilename = this._GetVirtualDiskFilename(volume.VmSCSIBus, volume.VmSCSIUnitNumber);
                this._AssertVolumeNotAttached(volume);
                this._Logger.FormatVerbose("The disk associated with filename \"{0}\" will be attached to bus {1} and unit {2}", (object)volume.VirtualDiskFilename, (object)volume.ApplianceSCSIBus, (object)volume.ApplianceSCSIUnitNumber);
            }
            this._Logger.Verbose("Preparation complete");
        }

        private void _GetInformation()
        {
            VmdkProperties[] replicaVmVmdkInfo = this._GetVmVMDKInfo();
            this._VirtualDiskInfo = new Dictionary<int, string>(replicaVmVmdkInfo.Length);
            foreach (VmdkProperties vmdkProperties in replicaVmVmdkInfo)
                this._VirtualDiskInfo.Add(this.SCSIKey(vmdkProperties.BusNumber, vmdkProperties.UnitNumber), vmdkProperties.FileName);
        }

        private void _VerifyConfiguration()
        {
            OculiServiceVolumeOptions[] sourceVolumes = this._Context.JobInfoWrapper.Volumes;
            if (this._VirtualDiskInfo.Count != sourceVolumes.Length)
                throw new OculiServiceServiceException(0, string.Format("Not enough drives attached to replica vm.  There were {0:d} drives originally specified, but {1:d} in the replica", (object)sourceVolumes.Length, (object)this._VirtualDiskInfo.Count));
        }

        private VmDiskInfo[] TakeFirstControllerDisks(List<VmDiskInfo> vmDiskInfos)
        {
            VmDiskInfo[] vmDiskInfoArray = (VmDiskInfo[])null;
            if (vmDiskInfos.Count > 0)
            {
                int controllerKey = vmDiskInfos[0].CtrlKey;
                vmDiskInfoArray = vmDiskInfos.Where<VmDiskInfo>((Func<VmDiskInfo, bool>)(diskInfo => diskInfo.CtrlKey == controllerKey)).ToArray<VmDiskInfo>();
                vmDiskInfos.RemoveAll((Predicate<VmDiskInfo>)(diskInfo => diskInfo.CtrlKey == controllerKey));
            }
            return vmDiskInfoArray;
        }

        protected string _GetVirtualDiskFilename(int bus, int unit)
        {
            string str;
            if (!this._VirtualDiskInfo.TryGetValue(this.SCSIKey(bus, unit), out str))
                throw new OculiServiceServiceException(0, "Unable to find internal volume information for disk on replica virtual device node SCSI(" + (object)bus + ", " + (object)unit + ")");
            return str;
        }

        private void _ConnectDrives()
        {
            this._Logger.Verbose("Connecting disks");
            List<VmDiskInfo> list = ((IEnumerable<OculiServiceVolumePersistedState>)this._Context.JobInfoWrapper.VolumePersistedState).Select<OculiServiceVolumePersistedState, VmDiskInfo>((Func<OculiServiceVolumePersistedState, VmDiskInfo>)(volumeInfo => new VmDiskInfo(true, volumeInfo.VirtualDiskFilename, this._GetSCSIControllerKeyFromHelperBus(volumeInfo), volumeInfo.ApplianceSCSIUnitNumber, 0L, "persistent", (IVimDatastore)null, volumeInfo.PreexistingDiskPath))).ToList<VmDiskInfo>();
            while (list.Count > 0)
            {
                VmDiskInfo[] diskBatch = this.TakeFirstControllerDisks(list);
                Dictionary<uint, DiskInformation> currentDiskInformation = this._GetCurrentDiskInformation();
                ConnectVirtualDiskESXCommon.LogDiskInformation(currentDiskInformation, this._Logger);
                CUtils.Retry(this._ConnectAttempts, this._ConnectDelay, (CUtils.Workload)(() => this._AddVirtualDisksToHelper(diskBatch)));
                this._Logger.Verbose("VMWare has been asked to add these disks:  " + string.Join(",", ((IEnumerable<VmDiskInfo>)diskBatch).Select<VmDiskInfo, string>((Func<VmDiskInfo, string>)(vmDiskInfo => vmDiskInfo.File)).ToArray<string>()));
                try
                {
                    Dictionary<uint, DiskInformation> newDisks = this._GetNewDisks(currentDiskInformation, diskBatch.Length);
                    int diskBatchSCSIBus = this._GetSCSIBusFromControllerKey(diskBatch);
                    foreach (OculiServiceVolumePersistedState volume in ((IEnumerable<OculiServiceVolumePersistedState>)this._Context.JobInfoWrapper.VolumePersistedState).Where<OculiServiceVolumePersistedState>((Func<OculiServiceVolumePersistedState, bool>)(v => v.ApplianceSCSIBus == diskBatchSCSIBus)))
                        this._FillVolumeInfoFromNewDisk(newDisks, volume);
                }
                catch (Exception ex)
                {
                    this._ResetESXConnection();
                    this._RemoveAllDrives();
                    throw;
                }
            }
        }

        protected virtual int _GetSCSIBusFromControllerKey(VmDiskInfo[] diskBatch)
        {
            return ESXSCSIHelper.GetSCSIBusFromControllerKey(this._Context.ESXHost.HelperVm(), diskBatch[0].CtrlKey);
        }

        protected virtual void _RemoveAllDrives()
        {
            this._Logger.Verbose("Removing any drives that got added as part of recovery process");
            this._Context.Invoker.RemoveAllDrives();
            this._RescanHardware();
        }

        protected virtual void _OnlineAllDisks()
        {
            this._Context.Invoker.OnlineAllDisks();
        }
    }
}
