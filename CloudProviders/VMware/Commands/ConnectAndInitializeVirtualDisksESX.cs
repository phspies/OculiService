using OculiService.Common.Logging;
using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;
using OculiService.Contract;

namespace OculiService.CloudProviders.VMware
{
  public class ConnectAndInitializeVirtualDisksESX : TaskCommand, ITaskCommandCommon, ITaskCommandBase
  {
    public ConnectAndInitializeVirtualDisksESX(TaskContext context)
      : base(context)
    {
    }

    public void Invoke()
    {
      this._Context.SetLowLevelState("ConfiguringAppliance");
      this._Logger.Information("Connecting replica vm's drives to the helper appliance.");
      this._ConnectAllVirtualDisks();
      this._InitializeAttachedDisks();
      this._Logger.Information("Finished connecting replica vm's drives to helper vm");
    }

    private void _InitializeAttachedDisks()
    {
      lock (Win32Utils.HelperOSLock)
      {
        foreach (OculiServiceVolumePersistedState item_0 in this._Context.JobInfoWrapper.VolumePersistedState)
        {
          this._CheckStopping();
          this._Context.SetLowLevelState("InitializingDisk");
          this._InitializeVirtualDisk(item_0);
        }
      }
    }

    private void _CheckStopping()
    {
      if (this._Context.StoppingProtection)
        throw new OculiServiceServiceException(0, "Job is stopping");
    }

    protected virtual void _InitializeVirtualDisk(OculiServiceVolumePersistedState tempVolumeInfo)
    {
      this._Context.Invoker.InitializeVirtualDisk(tempVolumeInfo);
    }

    protected virtual void _ConnectAllVirtualDisks()
    {
      this._Context.Invoker.ConnectAllVirtualDisks();
    }
  }
}
