using System.Net;

namespace OculiService.Common.Net
{
  public static class CredentialUtils
  {
    public static NetworkCredential Empty
    {
      get
      {
        return new NetworkCredential("", "", "");
      }
    }

    public static void Normalize(ref string userName, ref string domain)
    {
      Invariant.ArgumentNotNull((object) userName, "userName");
      if (!string.IsNullOrEmpty(domain))
        return;
      int length1 = userName.IndexOf('\\');
      if (length1 < 0)
      {
        int length2 = userName.IndexOf('@');
        if (length2 < 0)
        {
          domain = string.Empty;
        }
        else
        {
          domain = userName.Substring(length2 + 1);
          userName = userName.Substring(0, length2);
        }
      }
      else
      {
        domain = userName.Substring(0, length1);
        userName = userName.Substring(length1 + 1);
      }
    }
  }
}
