using Oculi.Commands.Interfaces;
using Oculi.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class EnableNicsESX : TaskCommand, IJobCommandConditional
  {
    public EnableNicsESX(TaskContext context)
      : base(context)
    {
    }

    public void Invoke(bool condition)
    {
      this._SetVmNicConnectivity(condition);
    }

    protected virtual void _SetVmNicConnectivity(bool testfailover)
    {
      this._Context.ESXHost.Vm().SetNicConnectivity(!testfailover, this._Context.ESXHost.ClientCtx);
    }
  }
}
