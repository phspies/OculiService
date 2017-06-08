namespace OculiService.Common
{
  public class SpecialCharacters
  {
    public static readonly char[] WinSpecialChars = new char[10]{ '#', '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
    public static readonly char[] WinSpecialCharsExcludePathSeperator = new char[9]{ '#', '\\', ':', '*', '?', '"', '<', '>', '|' };

    public static bool ContainSpecialChars(string text)
    {
      return text.IndexOfAny(SpecialCharacters.WinSpecialChars) != -1;
    }

    public static bool ContainSpecialCharsExcludePathSeperator(string text)
    {
      return text.IndexOfAny(SpecialCharacters.WinSpecialCharsExcludePathSeperator) != -1;
    }
  }
}
