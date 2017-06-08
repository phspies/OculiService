using OculiService.Common.Net;
using System;
using System.Net;
using System.Net.Sockets;

namespace OculiService.Common
{
  public class HostUriBuilder
  {
    private NetworkCredential credentials;
    private string _networkId;

    public string Scheme { get; set; }

    public string NetworkId
    {
      get
      {
        return this._networkId;
      }
      set
      {
        IPAddress address;
        if (IPAddress.TryParse(value, out address) && address.AddressFamily == AddressFamily.InterNetworkV6)
          this._networkId = address.ToString();
        else
          this._networkId = value;
      }
    }

    public int? Port { get; set; }

    public NetworkCredential Credentials
    {
      get
      {
        return this.credentials;
      }
      set
      {
        this.credentials = value ?? CredentialUtils.Empty;
      }
    }

    public string Path { get; set; }

    public string Query { get; set; }

    public string Fragment { get; set; }

    public Uri Uri
    {
      get
      {
        if (string.IsNullOrEmpty(this.Scheme))
          throw new InvalidOperationException("Unable to build a host URI with a null or empty Scheme");
        if (string.IsNullOrEmpty(this.NetworkId))
          throw new InvalidOperationException("Unable to build a host URI with a null or empty NetworkId");
        UriBuilder uriBuilder1 = new UriBuilder(this.Scheme, Uri.EscapeUriString(this.NetworkId));
        int? port = this.Port;
        if (port.HasValue)
        {
          UriBuilder uriBuilder2 = uriBuilder1;
          port = this.Port;
          int num = port.Value;
          uriBuilder2.Port = num;
        }
        uriBuilder1.Path = this.Path;
        uriBuilder1.Query = this.Query;
        uriBuilder1.Fragment = this.Fragment;
        if (!string.IsNullOrEmpty(this.Credentials.UserName))
        {
          string stringToEscape = this.Credentials.UserName;
          if (!string.IsNullOrEmpty(this.Credentials.Domain))
            stringToEscape = this.Credentials.Domain + "\\" + stringToEscape;
          uriBuilder1.UserName = Uri.EscapeDataString(stringToEscape);
          uriBuilder1.Password = Uri.EscapeDataString(this.Credentials.Password);
        }
        return uriBuilder1.Uri;
      }
    }

    public HostUriBuilder()
    {
      this.Scheme = "dtms";
      this.NetworkId = "localhost";
      this.credentials = CredentialUtils.Empty;
    }

    public HostUriBuilder(Uri uri)
    {
      this.ConstructFrom(new UriBuilder(uri));
    }

    public HostUriBuilder(string uriString)
    {
      this.ConstructFrom(new UriBuilder(uriString));
    }

    private void ConstructFrom(UriBuilder uriBuilder)
    {
      this.Scheme = uriBuilder.Scheme;
      this.NetworkId = Uri.UnescapeDataString(uriBuilder.Host);
      this.Path = uriBuilder.Path;
      this.Query = uriBuilder.Query;
      this.Fragment = uriBuilder.Fragment;
      if (!string.IsNullOrEmpty(uriBuilder.UserName))
      {
        string userName = Uri.UnescapeDataString(uriBuilder.UserName);
        string empty = string.Empty;
        if (userName.IndexOf('\\') != -1)
        {
          string[] strArray = userName.Split('\\');
          int index1 = 0;
          empty = strArray[index1];
          int index2 = 1;
          userName = strArray[index2];
        }
        string password = string.Empty;
        if (!string.IsNullOrEmpty(uriBuilder.Password))
          password = Uri.UnescapeDataString(uriBuilder.Password);
        this.credentials = new NetworkCredential(userName, password, empty);
      }
      else
        this.credentials = CredentialUtils.Empty;
      if (uriBuilder.Port == -1)
        return;
      this.Port = new int?(uriBuilder.Port);
    }
  }
}
