using System;using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection;

namespace OculiService.Common
{
  public class SmtpClientEx : SmtpClient
  {
    private static readonly FieldInfo localHostName = SmtpClientEx.GetLocalHostNameField();

    public string LocalHostName
    {
      get
      {
        if ((FieldInfo) null == SmtpClientEx.localHostName)
          return (string) null;
        return (string) SmtpClientEx.localHostName.GetValue((object) this);
      }
      set
      {
        if (string.IsNullOrEmpty(value))
          throw new ArgumentNullException("value");
        if (!((FieldInfo) null != SmtpClientEx.localHostName))
          return;
        SmtpClientEx.localHostName.SetValue((object) this, (object) value);
      }
    }

    public SmtpClientEx(string host, int port)
      : base(host, port)
    {
      this.Initialize();
    }

    public SmtpClientEx(string host)
      : base(host)
    {
      this.Initialize();
    }

    public SmtpClientEx()
    {
      this.Initialize();
    }

    private static FieldInfo GetLocalHostNameField()
    {
      FieldInfo field = typeof (SmtpClient).GetField("clientDomain", BindingFlags.Instance | BindingFlags.NonPublic);
      if ((FieldInfo) null == field)
        field = typeof (SmtpClient).GetField("localHostName", BindingFlags.Instance | BindingFlags.NonPublic);
      return field;
    }

    private void Initialize()
    {
      IPGlobalProperties globalProperties = IPGlobalProperties.GetIPGlobalProperties();
      if (string.IsNullOrEmpty(globalProperties.HostName) || string.IsNullOrEmpty(globalProperties.DomainName))
        return;
      this.LocalHostName = globalProperties.HostName + "." + globalProperties.DomainName;
    }
  }
}
