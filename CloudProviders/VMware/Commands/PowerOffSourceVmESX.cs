using OculiService.Common.Logging;
using System;
using VimWrapper;
using Oculi.Commands.Interfaces;

namespace OculiService.CloudProviders.VMware
{
  public class PowerOffSourceVmESX : JobCommand, IJobCommandCommon, IJobCommandBase
  {
    public PowerOffSourceVmESX(JobContext context)
      : base(context)
    {
    }

    public void Invoke()
    {
      try
      {
        IVimVm vimVm = this._SourceVm();
        if (vimVm == null || !vimVm.IsPoweredOn())
          return;
        vimVm.PowerOff(this._ClientCtx());
      }
      catch (Exception ex)
      {
        this._Logger.Warning(ex, "There was an exception powering off the VM, using backup strategy: ");
        this._ShutdownSource();
      }
    }

    protected virtual IVimVm _SourceVm()
    {
      return this._Context.ESXHost.SourceVm();
    }

    protected virtual VimClientlContext _ClientCtx()
    {
      return this._Context.ESXHost.ClientCtx;
    }

    protected virtual void _ShutdownSource()
    {
      this._Context.Engine.ShutdownSource();
    }
  }
}
