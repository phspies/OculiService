using System.Threading.Tasks;

namespace OculiService.Common.Tasks
{
  public class ActionResult
  {
    public ActivityToken Token { get; private set; }
    public Task Task { get; private set; }
    public ActionResult(ActivityToken token, Task task)
    {
      this.Token = token;
      this.Task = task;
    }
  }
}
