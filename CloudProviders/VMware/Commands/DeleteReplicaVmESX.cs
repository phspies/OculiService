using OculiService.Common.Logging;
using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class DeleteVmESX : TaskCommand, IJobCommandCommon, ITaskCommandBase
  {
    public DeleteVmESX(TaskContext context)
      : base(context)
    {
    }

    public void Invoke()
    {
      this._Logger.Information("Deleting  Vm from the ESX host.");
      this._DeleteVm();
      this._Logger.Information("Deleting  Vm Completed.");
    }

    protected virtual void _DeleteVm()
    {
      new DeleteVmOp(this._Context.ESXHost.VC_Vim, this._Context.ESXHost.HelperESXHost(), this._Context.ESXHost.Vm(), this._Context.Logger, 120).Run();
    }
  }
}
