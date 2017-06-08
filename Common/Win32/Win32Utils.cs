using OculiService.Common.Properties;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace OculiService.Win32
{
  [CLSCompliant(false)]
  public class Win32Utils
  {
    private const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 256;
    private const uint FORMAT_MESSAGE_IGNORE_INSERTS = 512;
    private const uint FORMAT_MESSAGE_FROM_SYSTEM = 4096;

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, string[] Arguments);

    [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2W", CharSet = CharSet.Unicode)]
    public static extern int WNetAddConnection2(ref Win32Utils.NETRESOURCE netResource, [MarshalAs(UnmanagedType.LPWStr)] string password, [MarshalAs(UnmanagedType.LPWStr)] string username, [MarshalAs(UnmanagedType.U4)] int flags);

    [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2W", CharSet = CharSet.Unicode)]
    public static extern int WNetCancelConnection2([MarshalAs(UnmanagedType.LPWStr)] string lpName, [MarshalAs(UnmanagedType.U4)] int dwFlags, [MarshalAs(UnmanagedType.Bool)] bool bForce);

    public static string NetAddConnection(string server, string username, string password)
    {
      Win32Utils.NETRESOURCE netResource = new Win32Utils.NETRESOURCE();
      netResource.dwDisplayType = 1;
      netResource.dwScope = 0;
      netResource.dwType = 0;
      netResource.dwUsage = 2;
      netResource.LocalName = "";
      netResource.RemoteName = Win32Utils.GetWNetServerString(server);
      netResource.Provider = (string) null;
      string username1;
      string domain;
      Win32Utils.SplitUsernameAndDomain(username, out username1, out domain);
      username = !string.IsNullOrEmpty(domain) ? username1 + "@" + domain : server.Replace(':', '-') + "\\" + username1;
      int num = Win32Utils.WNetAddConnection2(ref netResource, password, username, 0);
      string str;
      switch (num)
      {
        case 0:
        case 1219:
        case 71:
          str = (string) null;
          break;
        default:
          str = Win32Utils.FormatWin32ErrorMessage((uint) num);
          break;
      }
      return str;
    }

    public static string FormatWin32ErrorMessage(uint error)
    {
      IntPtr zero = IntPtr.Zero;
      string str;
      if ((int) Win32Utils.FormatMessage(4864U, IntPtr.Zero, error, 0U, ref zero, 0U, (string[]) null) == 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidServer, new object[1]
        {
          (object) error
        });
      else
        str = Marshal.PtrToStringAnsi(zero);
      return str;
    }

    public static void NetCancelConnection(string server)
    {
      Win32Utils.WNetCancelConnection2(Win32Utils.GetWNetServerString(server), 0, true);
    }

    public static string ValidateCredentials(string server, string username, string password)
    {
      try
      {
        return Win32Utils.NetAddConnection(server, username, password);
      }
      finally
      {
        Win32Utils.NetCancelConnection(server);
      }
    }

    public static string ValidateCredentials(string server, NetworkCredential credentials)
    {
      string fullUserName = Win32Utils.GetFullUserName(credentials);
      return Win32Utils.ValidateCredentials(server, fullUserName, credentials.Password);
    }

    public static string GetWNetServerString(string server)
    {
      IPAddress address = (IPAddress) null;
      string str;
      if (IPAddress.TryParse(server, out address))
      {
        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
          str = "\\\\" + server.Replace(":", "-") + ".ipv6-literal.net";
        }
        else
        {
          if (address.AddressFamily != AddressFamily.InterNetwork)
            throw new NotSupportedException(address.AddressFamily.ToString());
          str = "\\\\" + server;
        }
      }
      else
        str = "\\\\" + server;
      return str;
    }

    public static void SplitUsernameAndDomain(string fullUsername, out string username, out string domain)
    {
      if (fullUsername.Contains("\\"))
      {
        string[] strArray = fullUsername.Split("\\".ToCharArray());
        username = strArray[1];
        domain = strArray[0];
      }
      else if (fullUsername.Contains("@"))
      {
        string[] strArray = fullUsername.Split("@".ToCharArray());
        username = strArray[0];
        domain = strArray[1];
      }
      else
      {
        username = fullUsername;
        domain = string.Empty;
      }
    }

    public static string GetFullUserName(NetworkCredential credentials)
    {
      if (credentials == null)
        return string.Empty;
      string str = credentials.UserName;
      if (!string.IsNullOrEmpty(credentials.Domain))
        str = credentials.Domain + "\\" + credentials.UserName;
      return str;
    }

    public static void SplitServerNameAndPort(string server, out string serverName, out int port)
    {
      serverName = server;
      port = 0;
      if (server.Split(':').Length >= 3)
      {
        if (!server.Contains("[") || !server.Contains("]"))
          return;
        string[] strArray1 = server.Split("[]".ToCharArray());
        serverName = strArray1[1];
        if (!strArray1[2].Contains(":"))
          return;
        string[] strArray2 = strArray1[2].Split(':');
        port = int.Parse(strArray2[1]);
      }
      else
      {
        string[] strArray = server.Split(':');
        serverName = strArray[0];
        if (strArray.Length != 2)
          return;
        port = int.Parse(strArray[1]);
      }
    }

    public static string GetVimUuidFromBiosUuid(Guid uuid)
    {
      if (uuid == Guid.Empty)
        return (string) null;
      return Win32Utils.GetVimUuidFromBiosUuid(uuid.ToString());
    }

    public static string GetVimUuidFromBiosUuid(string vmUuid)
    {
      if (string.IsNullOrEmpty(vmUuid))
        return (string) null;
      string[] strArray = vmUuid.Split("-".ToCharArray());
      for (int index1 = 0; index1 < 3; ++index1)
      {
        string str1 = strArray[index1];
        string str2 = "";
        int index2 = str1.Length - 1;
        while (index2 >= 0)
        {
          string str3 = str2;
          char ch = str1[index2 - 1];
          string str4 = ch.ToString();
          string str5 = str3 + str4;
          ch = str1[index2];
          string str6 = ch.ToString();
          str2 = str5 + str6;
          index2 -= 2;
        }
        strArray[index1] = str2;
      }
      vmUuid = string.Join("-", strArray);
      return vmUuid;
    }

    public struct NETRESOURCE
    {
      [MarshalAs(UnmanagedType.U4)]
      public int dwScope;
      [MarshalAs(UnmanagedType.U4)]
      public int dwType;
      [MarshalAs(UnmanagedType.U4)]
      public int dwDisplayType;
      [MarshalAs(UnmanagedType.U4)]
      public int dwUsage;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string LocalName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string RemoteName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Comment;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Provider;
    }
  }
}
