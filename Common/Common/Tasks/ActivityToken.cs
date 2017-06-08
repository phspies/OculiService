using System;

namespace OculiService.Common.Tasks
{
  public class ActivityToken
  {
    public Guid Id { get; set; }
    public string ActivityNameId { get; set; }
    public string[] ActivityNameFormatParameters { get; set; }
  }
}
