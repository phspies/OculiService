using System;
namespace OculiService.Common.Threading
{
  public sealed class Deferral : IDisposable
  {
    private DeferrableOperation deferrableOperation;

    internal Deferral(DeferrableOperation deferrableOperation)
    {
      this.deferrableOperation = deferrableOperation;
    }

    public void Complete()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      if (this.deferrableOperation == null)
        return;
      this.deferrableOperation.Release();
      this.deferrableOperation = (DeferrableOperation) null;
    }
  }
}
