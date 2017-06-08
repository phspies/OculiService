using System;using System.Net;
using System.Net.NetworkInformation;

namespace OculiService.Common.Net.NetworkInformation
{
  public class PingWrapper : IPing, IDisposable
  {
    private Ping ping = new Ping();

    public event PingCompletedEventHandler PingCompleted
    {
      add
      {
        this.ping.PingCompleted += value;
      }
      remove
      {
        this.ping.PingCompleted -= value;
      }
    }

    public PingReply Send(IPAddress address)
    {
      return this.ping.Send(address);
    }

    public PingReply Send(string hostNameOrAddress)
    {
      return this.ping.Send(hostNameOrAddress);
    }

    public PingReply Send(IPAddress address, int timeout)
    {
      return this.ping.Send(address, timeout);
    }

    public PingReply Send(string hostNameOrAddress, int timeout)
    {
      return this.ping.Send(hostNameOrAddress, timeout);
    }

    public PingReply Send(IPAddress address, int timeout, byte[] buffer)
    {
      return this.ping.Send(address, timeout, buffer);
    }

    public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer)
    {
      return this.ping.Send(hostNameOrAddress, timeout, buffer);
    }

    public PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
    {
      return this.ping.Send(address, timeout, buffer, options);
    }

    public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
    {
      return this.ping.Send(hostNameOrAddress, timeout, buffer, options);
    }

    public void SendAsync(string hostNameOrAddress, object userToken)
    {
      this.ping.SendAsync(hostNameOrAddress, userToken);
    }

    public void SendAsync(IPAddress address, int timeout, object userToken)
    {
      this.ping.SendAsync(address, timeout, userToken);
    }

    public void SendAsync(string hostNameOrAddress, int timeout, object userToken)
    {
      this.ping.SendAsync(hostNameOrAddress, timeout, userToken);
    }

    public void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken)
    {
      this.ping.SendAsync(address, timeout, buffer, userToken);
    }

    public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
    {
      this.ping.SendAsync(hostNameOrAddress, timeout, buffer, userToken);
    }

    public void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
    {
      this.ping.SendAsync(address, timeout, buffer, options, userToken);
    }

    public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
    {
      this.ping.SendAsync(hostNameOrAddress, timeout, buffer, options, userToken);
    }

    public void SendAsyncCancel()
    {
      this.ping.SendAsyncCancel();
    }

    public void Dispose()
    {
      this.ping.Dispose();
    }
  }
}
