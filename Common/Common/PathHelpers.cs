using System;using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OculiService.Common
{
  public static class PathHelpers
  {
    public static string GetFullPath(string relativePath, RelativeFolder baseLocation)
    {
      return PathHelpers.GetFullPath(relativePath, baseLocation, RelativeSubfolder.None);
    }

    public static string GetFullPath(string relativePath, RelativeFolder baseLocation, RelativeSubfolder subfolder)
    {
      if (Path.IsPathRooted(relativePath) || baseLocation == RelativeFolder.Current)
        return Path.GetFullPath(relativePath);
      string str;
      if (baseLocation == RelativeFolder.Application)
      {
        Assembly assembly = Assembly.GetEntryAssembly();
        if ((object) assembly == null)
          assembly = Assembly.GetExecutingAssembly();
        str = Path.GetDirectoryName(assembly.Location);
      }
      else
      {
        str = Environment.GetFolderPath((Environment.SpecialFolder) (baseLocation & (RelativeFolder) 255));
        if ((baseLocation & (RelativeFolder) 256) == (RelativeFolder) 256)
          str = PathHelpers.AddProductPath(str);
      }
      if (subfolder != RelativeSubfolder.None)
        str = Path.Combine(str, Enum.GetName(subfolder.GetType(), (object) subfolder));
      return Path.GetFullPath(Path.Combine(str, relativePath));
    }

    private static string AddProductPath(string rootPath)
    {
      Assembly assembly1 = Assembly.GetEntryAssembly();
      if ((object) assembly1 == null)
        assembly1 = Assembly.GetExecutingAssembly();
      Assembly assembly2 = assembly1;
      AssemblyCompanyAttribute assemblyAttribute1 = PathHelpers.GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly2);
      string path2_1 = assemblyAttribute1 == null ? assembly2.GetName().Name : assemblyAttribute1.Company;
      AssemblyProductAttribute assemblyAttribute2 = PathHelpers.GetAssemblyAttribute<AssemblyProductAttribute>(assembly2);
      string path2_2 = assemblyAttribute2 == null ? assembly2.GetName().Name : assemblyAttribute2.Product;
      return Path.Combine(Path.Combine(rootPath, path2_1), path2_2);
    }

    private static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
    {
      T[] customAttributes = (T[]) assembly.GetCustomAttributes(typeof (T), true);
      if (customAttributes != null && customAttributes.Length != 0)
        return customAttributes[0];
      return default (T);
    }

    public static bool IsRoot(string path)
    {
      return Regex.IsMatch(path, "^.+:\\\\$");
    }

    public static bool IsRooted(string path)
    {
      return Regex.IsMatch(path, "^.+:\\\\");
    }

    public static string GetDirectoryAtDepth(string path, int depth)
    {
      int startIndex = 0;
      if (PathHelpers.IsRooted(path))
        startIndex = path.IndexOf("\\") + 1;
      string[] strArray = path.Substring(startIndex).Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      if (depth < 0 || depth > strArray.Length - 1)
        return (string) null;
      return strArray[depth];
    }
  }
}
