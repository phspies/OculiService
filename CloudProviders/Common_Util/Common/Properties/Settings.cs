// Decompiled with JetBrains decompiler
// Type: Common_Util.Properties.Settings
// Assembly: Oculi.Virtualization.Common_Util, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 729775AE-A0F3-4545-B89A-1AD01077DBC4
// Assembly location: C:\Downloads\Double-Take\Service\Oculi.Virtualization.Common_Util.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Common_Util.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        return Settings.defaultInstance;
      }
    }

    [ApplicationScopedSetting]
    [DebuggerNonUserCode]
    [SpecialSetting(SpecialSetting.WebServiceUrl)]
    [DefaultSettingValue("http://updates.Oculi.com/update.asmx")]
    public string Common_Util_com_Oculi_updates_update
    {
      get
      {
        return (string) this["Common_Util_com_Oculi_updates_update"];
      }
    }

    private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
    {
    }

    private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
    {
    }
  }
}
