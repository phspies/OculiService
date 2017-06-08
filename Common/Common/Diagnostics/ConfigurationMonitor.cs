using System;using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace OculiService.Common.Diagnostics
{
  public static class ConfigurationMonitor
  {
    private static readonly FileSystemWatcher[] watchers = ConfigurationMonitor.GetWatchers().ToArray<FileSystemWatcher>();

    public static bool Enabled
    {
      get
      {
        return ConfigurationMonitor.watchers[0].EnableRaisingEvents;
      }
      set
      {
        foreach (FileSystemWatcher watcher in ConfigurationMonitor.watchers)
          watcher.EnableRaisingEvents = value;
      }
    }

    public static event EventHandler ConfigurationChanged;

    private static IEnumerable<FileSystemWatcher> GetWatchers()
    {
      return ConfigurationMonitor.GetConfigurationFilePaths().Where<string>((Func<string, bool>) (p =>
      {
        if (p != null)
          return Directory.Exists(p);
        return false;
      })).Select<string, FileSystemWatcher>((Func<string, FileSystemWatcher>) (p => ConfigurationMonitor.CreateWatcher(p)));
    }

    private static FileSystemWatcher CreateWatcher(string path)
    {
      try
      {
        string directoryName = Path.GetDirectoryName(path);
        string fileName = Path.GetFileName(path);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(directoryName, fileName);
        fileSystemWatcher.EnableRaisingEvents = false;
        FileSystemEventHandler systemEventHandler = new FileSystemEventHandler(ConfigurationMonitor.OnWatcherChanged);
        fileSystemWatcher.Changed += systemEventHandler;
        return fileSystemWatcher;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private static IEnumerable<string> GetConfigurationFilePaths()
    {
      yield return ConfigurationMonitor.GetConfigurationPath(ConfigurationUserLevel.None);
      yield return ConfigurationMonitor.GetConfigurationPath(ConfigurationUserLevel.PerUserRoaming);
      yield return ConfigurationMonitor.GetConfigurationPath(ConfigurationUserLevel.PerUserRoamingAndLocal);
    }

    private static string GetConfigurationPath(ConfigurationUserLevel level)
    {
      try
      {
        return ConfigurationManager.OpenExeConfiguration(level).FilePath;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    private static void OnWatcherChanged(object sender, FileSystemEventArgs args)
    {
      
      EventHandler configurationChanged = ConfigurationMonitor.ConfigurationChanged;
      if (configurationChanged == null)
        return;
      configurationChanged((object) null, EventArgs.Empty);
    }
  }
}
