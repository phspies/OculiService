using System.Collections.Generic;

namespace Common_Util
{
  public static class DiskInformationExtensions
  {
    public static IDictionary<T, U> ExceptWith<T, U>(this IDictionary<T, U> d, IDictionary<T, U> e)
    {
      foreach (KeyValuePair<T, U> keyValuePair in (IEnumerable<KeyValuePair<T, U>>) e)
        d.Remove(keyValuePair.Key);
      return d;
    }
  }
}
