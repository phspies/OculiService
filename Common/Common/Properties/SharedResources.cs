using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace OculiService.Common.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class SharedResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (SharedResources.resourceMan == null)
          SharedResources.resourceMan = new ResourceManager("OculiService.Common.Properties.SharedResources", typeof (SharedResources).Assembly);
        return SharedResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get
      {
        return SharedResources.resourceCulture;
      }
      set
      {
        SharedResources.resourceCulture = value;
      }
    }

    public static string SourceQualificationFaultCommunication
    {
      get
      {
        return SharedResources.ResourceManager.GetString("SourceQualificationFaultCommunication", SharedResources.resourceCulture);
      }
    }

    public static string SourceQualificationFaultNatSourceNotSupported
    {
      get
      {
        return SharedResources.ResourceManager.GetString("SourceQualificationFaultNatSourceNotSupported", SharedResources.resourceCulture);
      }
    }

    public static string SourceQualificationFaultWrongSource
    {
      get
      {
        return SharedResources.ResourceManager.GetString("SourceQualificationFaultWrongSource", SharedResources.resourceCulture);
      }
    }

    internal SharedResources()
    {
    }
  }
}
