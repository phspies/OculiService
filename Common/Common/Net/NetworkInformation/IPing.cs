using System;using System.Net;
using System.Net.NetworkInformation;

namespace OculiService.Common.Net.NetworkInformation
{
  public interface IPing : IDisposable
  {
    event PingCompletedEventHandler PingCompleted;

    PingReply Send(IPAddress address);

    PingReply Send(string hostNameOrAddress);

    PingReply Send(IPAddress address, int timeout);

    PingReply Send(string hostNameOrAddress, int timeout);

    PingReply Send(IPAddress address, int timeout, byte[] buffer);

    PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer);

    PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options);

    PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options);

    void SendAsync(string hostNameOrAddress, object userToken);

    void SendAsync(IPAddress address, int timeout, object userToken);

    void SendAsync(string hostNameOrAddress, int timeout, object userToken);

    void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken);

    void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken);

    void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken);

    void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken);

    void SendAsyncCancel();
  }
}
