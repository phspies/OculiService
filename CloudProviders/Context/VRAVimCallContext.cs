using VimWrapper;

namespace Oculi.Jobs.Context
{
  public class OculiVimCallContext : VimClientlContext
  {
    public override int TimeoutSec
    {
      get
      {
        return 900;
      }
    }

    public override bool IsVimClientStopping()
    {
      return false;
    }
  }
}
