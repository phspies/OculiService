using OculiService.Common.Logging;
using System;
using VimWrapper;
using Oculi.Commands.Interfaces;

namespace OculiService.CloudProviders.VMware
{
  public class PowerOffVmESX : JobCommand, IJobCommandCommon, IJobCommandBase
  {
    public PowerOffVmESX(JobContext context)
      : base(context)
    {
    }

    public void Invoke()
    {
      try
      {
        IVimVm replicaVm = this._Vm();
        if (replicaVm == null || !replicaVm.IsPoweredOn())
          return;
        this._StopVm(replicaVm, this._VC(), this._Context.Logger, 120);
      }
      catch (Exception ex)
      {
        this._Logger.Warning(ex, "Exception caught powering down the replica.  It may be down already and that's ok.  Exception:");
      }
    }

    protected virtual IVimVm _Vm()
    {
      return this._Context.ESXHost.Vm();
    }

    protected virtual void _StopVm(IVimVm replicaVm, IVimService vc, ILogger logger, int maxRetryTime)
    {
      new StopVmOp(replicaVm, vc, logger, maxRetryTime).Run();
    }

    protected virtual IVimService _VC()
    {
      return this._Context.ESXHost.VC_Vim;
    }
  }
}
