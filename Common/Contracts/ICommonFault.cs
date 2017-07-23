
namespace OculiService.Common.Contract
{
  public interface ICommonFault
  {
    string Name { get; set; }

    string Message { get; set; }

    string StackTrace { get; set; }

    ICommonFault InnerFault { get; set; }
  }
}
