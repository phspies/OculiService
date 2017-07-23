using Oculi.Commands.Interfaces;
using Oculi.Jobs.Commands;
using Oculi.Jobs.Context;
using VimWrapper;

namespace OculiService.CloudProviders.VMware
{
  public class StartupESXVm : TaskCommand, ITaskCommandCommon, ITaskCommandBase
  {
    protected int _IterationCount = 6;
    protected int _TimeBetweenRetries = 5000;

    public StartupESXVm(TaskContext context) : base(context)
    {
    }

    public void Invoke()
    {
      CUtils.Retry(this._IterationCount, this._TimeBetweenRetries, (CUtils.Workload) (() => this._Vm().PowerOn(this._ClientCtx())));
    }

    protected virtual IVimVm _Vm()
    {
      return this._Context.ESXHost.Vm();
    }

    protected virtual VimClientlContext _ClientCtx()
    {
      return this._Context.ESXHost.ClientCtx;
    }
  }
}
