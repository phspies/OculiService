using System.Diagnostics;
using System.Net;
using System.Security;

namespace OculiService.Common.Diagnostics
{
  public static class ProcessFactoryExtensions
  {
    public static IProcess CreateProcess(this IProcessFactory factory, string fileName)
    {
      return factory.CreateProcess(new ProcessStartInfo(fileName));
    }

    public static IProcess CreateProcess(this IProcessFactory factory, string fileName, string arguments)
    {
      return factory.CreateProcess(new ProcessStartInfo(fileName, arguments));
    }

    public static IProcess CreateProcess(this IProcessFactory factory, string fileName, string userName, SecureString password, string domain)
    {
      return factory.CreateProcess(fileName, (string) null, userName, password, domain);
    }

    public static IProcess CreateProcess(this IProcessFactory factory, string fileName, string arguments, string userName, SecureString password, string domain)
    {
      Invariant.ArgumentNotNull((object) factory, "factory");
      Invariant.ArgumentNotNullOrEmpty(fileName, "fileName");
      return factory.CreateProcess(new ProcessStartInfo(fileName, arguments) { UseShellExecute = false, UserName = userName, Password = password, Domain = domain });
    }

    public static IProcess CreateProcess(this IProcessFactory factory, string fileName, NetworkCredential credentials)
    {
      return factory.CreateProcess(fileName, (string) null, credentials);
    }

    public static IProcess CreateProcess(this IProcessFactory factory, string fileName, string arguments, NetworkCredential credentials)
    {
      Invariant.ArgumentNotNull((object) factory, "factory");
      Invariant.ArgumentNotNullOrEmpty(fileName, "fileName");
      Invariant.ArgumentNotNull((object) credentials, "credentials");
      return factory.CreateProcess(fileName, arguments, credentials.UserName, ProcessFactoryExtensions.SecurePassword(credentials.Password), credentials.Domain);
    }

    public static SecureString SecurePassword(string password)
    {
      SecureString secureString = new SecureString();
      foreach (char c in password.ToCharArray())
        secureString.AppendChar(c);
      return secureString;
    }
  }
}
