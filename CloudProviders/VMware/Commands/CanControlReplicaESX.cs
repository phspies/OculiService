using VimWrapper;
using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class CanControlESX : TaskCommand, ITestCommandCommon
  {
    public CanControlESX(TaskContext context)
      : base(context)
    {
    }

    public bool Invoke()
    {
      return this._GetVm() != null;
    }

    protected virtual IVimVm _GetVm()
    {
      return this._Context.ESXHost.Vm();
    }
  }
}
