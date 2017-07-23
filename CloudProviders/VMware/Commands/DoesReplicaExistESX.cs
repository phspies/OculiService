using System.Collections.Generic;
using VimWrapper;
using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class DoesExistESX : TaskCommand, ITestCommandCommon
  {
    public DoesExistESX(TaskContext context)
      : base(context)
    {
    }

    public bool Invoke()
    {
      if (this._GetAllVmsIndexedByName().ContainsKey(this._Context.JobInfoWrapper.VmName))
        return true;
      IVimDatastore replicaVmDatastore = this._GetVmDatastore();
      VimClientlContext vimClientlContext = (VimClientlContext) new OculiServiceVimCallContext();
      string replicaVmName = this._Context.JobInfoWrapper.VmName;
      VimClientlContext ctx = vimClientlContext;
      return replicaVmDatastore.IsFolderOnRootExist(replicaVmName, ctx);
    }

    protected virtual IVimDatastore _GetVmDatastore()
    {
      return this._Context.ESXHost.VC_Vim.GetDatastoreByUrl(this._Context.JobInfoWrapper.DataStoreUrl);
    }

    protected virtual Dictionary<string, IVimVm> _GetAllVmsIndexedByName()
    {
      return this._Context.ESXHost.VC_Vim.GetAllVMsDictWithName();
    }
  }
}
