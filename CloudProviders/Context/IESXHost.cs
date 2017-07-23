using OculiService.CloudProviders.VMware;

namespace Oculi.Jobs.Context
{
  public interface IESXHost
  {
    VimClientlContext ClientCtx { get; set; }

    int ESXVersion_Major { get; }

    int ESXVersion_Minor { get; }

    IVimService VC_Vim { get; set; }

    IVimService Source_VC_Vim { get; set; }

    IVimHost HelperESXHost();

    IVimHost SourceESXHost();

    IVimVm HelperVm();

    IVimVm Vm();

    IVimVm SourceVm();

    string GetTargetDsName();
  }
}
