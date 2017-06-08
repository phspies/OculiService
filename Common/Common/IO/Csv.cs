using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OculiService.Common.IO
{
  public static class Csv
  {
    public static string FormatRow(IEnumerable<string> fields)
    {
      Invariant.ArgumentNotNull((object) fields, "fields");
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string field in fields)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(',');
        if (field.IndexOfAny(new char[2]{ '"', ',' }) != -1)
          stringBuilder.AppendFormat("\"{0}\"", (object) field.Replace("\"", "\"\""));
        else
          stringBuilder.Append(field);
      }
      return stringBuilder.ToString();
    }

    public static IEnumerable<string> ParseRow(string text)
    {
      Invariant.ArgumentNotNull((object) text, "text");
      return Csv.ParseRow((TextReader) new StringReader(text));
    }

    public static IEnumerable<string> ParseRow(TextReader stream)
    {
      string line = Csv.ReadLine(stream);
      if (line != null)
      {
        int pos = 0;
        while (pos < line.Length)
        {
          string str1;
          if ((int) line[pos] == 34)
          {
            ++pos;
            string str2 = string.Empty;
            int startIndex = pos;
            while (pos < line.Length)
            {
              if ((int) line[pos] == 34)
              {
                ++pos;
                if (pos >= line.Length)
                {
                  string str3 = Csv.ReadLine(stream);
                  if (str3 == null)
                  {
                    --pos;
                    break;
                  }
                  str2 = str2 + line.Substring(startIndex) + "\n";
                  line = str3;
                  pos = startIndex = 0;
                  continue;
                }
                if ((int) line[pos] != 34)
                {
                  --pos;
                  break;
                }
              }
              ++pos;
              if (pos >= line.Length)
              {
                string str3 = Csv.ReadLine(stream);
                if (str3 != null)
                {
                  str2 = str2 + line.Substring(startIndex) + "\n";
                  line = str3;
                  pos = startIndex = 0;
                }
                else
                  break;
              }
            }
            str1 = (str2 + line.Substring(startIndex, pos - startIndex)).Replace("\"\"", "\"");
          }
          else
          {
            int startIndex = pos;
            while (pos < line.Length && (int) line[pos] != 44)
              ++pos;
            str1 = line.Substring(startIndex, pos - startIndex);
          }
          yield return str1;
          while (pos < line.Length && (int) line[pos] != 44)
            ++pos;
          if (pos < line.Length)
            ++pos;
        }
      }
    }

    private static string ReadLine(TextReader stream)
    {
      string str;
      do
      {
        str = stream.ReadLine();
        if (str == null)
          return (string) null;
      }
      while (str.Trim().Length == 0);
      return str;
    }
  }
}
