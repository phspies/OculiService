// Decompiled with JetBrains decompiler
// Type: Common_Util.Properties.Resource
// Assembly: Oculi.Virtualization.Common_Util, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 729775AE-A0F3-4545-B89A-1AD01077DBC4
// Assembly location: C:\Downloads\Double-Take\Service\Oculi.Virtualization.Common_Util.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Common_Util.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resource
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Resource.resourceMan == null)
          Resource.resourceMan = new ResourceManager("Common_Util.Properties.Resource", typeof (Resource).Assembly);
        return Resource.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return Resource.resourceCulture;
      }
      set
      {
        Resource.resourceCulture = value;
      }
    }

    internal static string InvalidServer
    {
      get
      {
        return Resource.ResourceManager.GetString("InvalidServer", Resource.resourceCulture);
      }
    }

    internal static string InvalidUsernamePassword
    {
      get
      {
        return Resource.ResourceManager.GetString("InvalidUsernamePassword", Resource.resourceCulture);
      }
    }

    internal static string SizeInGB
    {
      get
      {
        return Resource.ResourceManager.GetString("SizeInGB", Resource.resourceCulture);
      }
    }

    internal static string SizeInMB
    {
      get
      {
        return Resource.ResourceManager.GetString("SizeInMB", Resource.resourceCulture);
      }
    }

    internal static string ValueInGHz
    {
      get
      {
        return Resource.ResourceManager.GetString("ValueInGHz", Resource.resourceCulture);
      }
    }

    internal static string ValueInMHz
    {
      get
      {
        return Resource.ResourceManager.GetString("ValueInMHz", Resource.resourceCulture);
      }
    }

    internal Resource()
    {
    }
  }
}
