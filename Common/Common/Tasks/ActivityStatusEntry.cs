using System;
using System.Runtime.Serialization;

namespace OculiService.Common.Tasks
{
  public class ActivityStatusEntry
  {
    public string MessageId { get; set; }
    public string[] MessageFormatParameters { get; set; }
    public ActivityToken Token { get; set; }
    public ActivityCompletionStatus Status { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public TimeSpan Duration { get; set; }
    public string RequesterUserName { get; set; }
  }
}
