using System;
using System.Net;

namespace OculiService.Common
{
  public static class UriExtensions
  {
    public static Uri StripCredentials(this Uri hostUri)
    {
      if (hostUri == (Uri) null)
        return (Uri) null;
      return new HostUriBuilder(hostUri) { Credentials = ((NetworkCredential) null) }.Uri;
    }
  }
}
