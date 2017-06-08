using System;using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace OculiService.Common.Net
{
  public static class IPEndPointExtensions
  {
    public static bool TryParse(string endpointString, out IPEndPoint endpoint)
    {
      endpoint = IPEndPointExtensions.InternalParse(endpointString, true);
      return endpoint != null;
    }

    public static IPEndPoint Parse(string endpointString)
    {
      return IPEndPointExtensions.InternalParse(endpointString, false);
    }

    public static IEnumerable<IPEndPoint> ToEndpoints(this IEnumerable<string> endpoints)
    {
      foreach (string endpoint1 in endpoints)
      {
        IPEndPoint endpoint2;
        if (IPEndPointExtensions.TryParse(endpoint1, out endpoint2))
          yield return endpoint2;
      }
    }

    private static IPEndPoint InternalParse(string endpointString, bool tryParse)
    {
      Invariant.ArgumentNotNull((object) endpointString, "endpointString");
      string[] strArray = endpointString.Split(':');
      if (strArray.Length == 1)
      {
        if (tryParse)
          return (IPEndPoint) null;
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid IP endpoint format: '{0}'", new object[1]{ (object) endpointString }));
      }
      string ipString;
      string s;
      if (strArray.Length == 2)
      {
        ipString = strArray[0];
        s = strArray[1];
      }
      else
      {
        ipString = string.Join(":", ((IEnumerable<string>) strArray).Take<string>(strArray.Length - 1).ToArray<string>());
        s = strArray[strArray.Length - 1];
      }
      IPAddress address;
      try
      {
        address = IPAddress.Parse(ipString);
      }
      catch (FormatException ex)
      {
        if (tryParse)
          return (IPEndPoint) null;
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid IP endpoint format: '{0}'", new object[1]{ (object) endpointString }), (Exception) ex);
      }
      int result;
      if (int.TryParse(s, out result) && result >= 0 && result <= (int) ushort.MaxValue)
        return new IPEndPoint(address, result);
      if (tryParse)
        return (IPEndPoint) null;
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid IP endpoint port: '{0}'", new object[1]{ (object) s }));
    }
  }
}
