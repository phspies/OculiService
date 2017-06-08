using System.Runtime.Serialization;

namespace OculiService.Common.Tasks
{
  public enum ActivityCompletionStatus
  {
    Pending,
    Running,
    Completed,
    Canceled,
    Faulted,
  }
}
