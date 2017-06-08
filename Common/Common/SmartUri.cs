using System;using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace OculiService.Common
{
  public class SmartUri
  {
    public const char PortDelimiter = ':';
    public const char IPv6BeginDelimiter = '[';
    public const char IPv6EndDelimiter = ']';
    private string _host;

    public string Host
    {
      get
      {
        return this._host;
      }
      private set
      {
        this._host = this.NormalizeHost(value);
      }
    }

    public string DnsFriendlyHost { get; private set; }

    public int Port { get; private set; }

    public string Authority
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) this.Host, (object) ':', (object) this.Port);
      }
    }

    public IPEndPoint IPEndPoint { get; private set; }

    public UriHostNameType HostNameType { get; private set; }

    private SmartUri()
    {
    }

    public SmartUri(IPEndPoint endpoint)
    {
      if (endpoint == null)
        throw new ArgumentNullException("endpoint");
      this.HostNameType = SmartUri.GetHostNameType(endpoint.Address);
      this.Host = endpoint.Address.ToString();
      string str;
      if (this.HostNameType != UriHostNameType.IPv6)
        str = this.Host;
      else
        str = this.Host.Trim('[', ']');
      this.DnsFriendlyHost = str;
      this.Port = endpoint.Port;
      this.IPEndPoint = endpoint;
    }

    public override string ToString()
    {
      return this.Authority;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      SmartUri smartUri = obj as SmartUri;
      if (smartUri != null)
      {
        if (this.HostNameType == UriHostNameType.Unknown || this.HostNameType != smartUri.HostNameType)
          return false;
        if (this.HostNameType == UriHostNameType.Dns)
          return this.Host.Equals(smartUri.Host, StringComparison.OrdinalIgnoreCase);
        return this.IPEndPoint.Equals((object) smartUri.IPEndPoint);
      }
      IPEndPoint ipEndPoint = obj as IPEndPoint;
      if (ipEndPoint != null && this.IPEndPoint != null)
        return this.IPEndPoint.Equals((object) ipEndPoint);
      return false;
    }

    public override int GetHashCode()
    {
      return this.IPEndPoint.GetHashCode();
    }

    public bool HostEquals(SmartUri hostUri)
    {
      if (hostUri == null || this.HostNameType == UriHostNameType.Unknown || this.HostNameType != hostUri.HostNameType)
        return false;
      if (this.HostNameType == UriHostNameType.Dns)
        return this.Host.Equals(hostUri.Host, StringComparison.OrdinalIgnoreCase);
      return this.IPEndPoint.Address.Equals((object) hostUri.IPEndPoint.Address);
    }

    public bool HostEquals(IPEndPoint hostEndpoint)
    {
      if (hostEndpoint == null || this.IPEndPoint == null)
        return false;
      return this.IPEndPoint.Address.Equals((object) hostEndpoint.Address);
    }

    public bool HostEquals(IPAddress hostAddress)
    {
      if (hostAddress == null || this.IPEndPoint == null)
        return false;
      return this.IPEndPoint.Address.Equals((object) hostAddress);
    }

    public bool HostEquals(string hostName)
    {
      if (string.IsNullOrEmpty(hostName))
        return false;
      return this.Host.Equals(hostName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryParse(string value, int defaultPort, out SmartUri uri)
    {
      uri = new SmartUri();
      if (string.IsNullOrEmpty(value))
        return false;
      int startIndex1 = value.IndexOf('[');
      int startIndex2 = 0;
      int length1 = value.Length;
      int num;
      if (startIndex1 >= 0)
      {
        startIndex2 = startIndex1;
        int startIndex3 = value.IndexOf(']', startIndex1);
        if (startIndex3 < 0)
          return false;
        length1 = startIndex3 - startIndex1 + 1;
        num = value.IndexOf(':', startIndex3);
        if (num > 0 && startIndex3 + 1 != num)
          return false;
      }
      else
      {
        num = value.LastIndexOf(':');
        if (num >= 0)
        {
          if (value.IndexOf(':') == num)
            length1 = num - startIndex2;
          else
            num = -1;
        }
      }
      string ipString = value.Substring(startIndex2, length1);
      IPAddress address;
      IPAddress.TryParse(ipString, out address);
      int result;
      if (num >= 0)
      {
        int startIndex3 = num + 1;
        int length2 = value.Length - startIndex3;
        if (!int.TryParse(value.Substring(startIndex3, length2), out result))
          return false;
      }
      else
        result = defaultPort;
      UriHostNameType hostNameType = SmartUri.GetHostNameType(address);
      
      
      SmartUri local = @uri;
      SmartUri smartUri1 = new SmartUri();
      smartUri1.HostNameType = hostNameType;
      smartUri1.Host = ipString;
      SmartUri smartUri2 = smartUri1;
      string str;
      if (hostNameType != UriHostNameType.IPv6)
        str = ipString;
      else
        str = ipString.Trim('[', ']');
      smartUri2.DnsFriendlyHost = str;
      smartUri1.Port = result;
      smartUri1.IPEndPoint = address != null ? new IPEndPoint(address, result) : (IPEndPoint) null;
      SmartUri smartUri3 = smartUri1;
      
      local = smartUri3;
      return true;
    }

    public static SmartUri Parse(string value, int defaultPort)
    {
      SmartUri uri;
      if (!SmartUri.TryParse(value, defaultPort, out uri))
        throw new FormatException("The string could not be parsed as a URI.");
      return uri;
    }

    public static SmartUri ParseOrDefault(string value, int defaultPort)
    {
      SmartUri uri;
      SmartUri.TryParse(value, defaultPort, out uri);
      return uri;
    }

    private static UriHostNameType GetHostNameType(IPAddress address)
    {
      if (address == null)
        return UriHostNameType.Dns;
      if (address.AddressFamily == AddressFamily.InterNetwork)
        return UriHostNameType.IPv4;
      return address.AddressFamily == AddressFamily.InterNetworkV6 ? UriHostNameType.IPv6 : UriHostNameType.Unknown;
    }

    private string NormalizeHost(string host)
    {
      IPAddress address;
      if (host.StartsWith('['.ToString()) || !IPAddress.TryParse(host, out address) || AddressFamily.InterNetworkV6 != address.AddressFamily)
        return host;
      return string.Format("{0}{1}{2}", (object) '[', (object) address, (object) ']');
    }
  }
}
