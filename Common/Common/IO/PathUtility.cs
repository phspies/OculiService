using System.IO;

namespace OculiService.Common.IO
{
  public static class PathUtility
  {
    private static readonly char[] invalidPathChars = Path.GetInvalidPathChars();
    private static readonly char[] wildcards = new char[2]{ '?', '*' };

    public static bool IsPathValid(string path)
    {
      if (!string.IsNullOrEmpty(path) && path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
        return path.IndexOfAny(PathUtility.wildcards) == -1;
      return false;
    }
  }
}
