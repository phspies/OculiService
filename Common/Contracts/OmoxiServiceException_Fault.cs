using OculiService.Common.Contract;
using System.Runtime.Serialization;

namespace OculiService.Virtualization.Contract
{
  public sealed class OculiServiceServiceException_Fault : ICommonFault
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string StackTrace { get; set; }

    [DataMember]
    public ICommonFault InnerFault { get; set; }
  }
}
