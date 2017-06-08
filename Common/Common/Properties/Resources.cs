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
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (OculiService.Common.Properties.Resources.resourceMan == null)
          OculiService.Common.Properties.Resources.resourceMan = new ResourceManager("OculiService.Common.Properties.Resources", typeof (OculiService.Common.Properties.Resources).Assembly);
        return OculiService.Common.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return OculiService.Common.Properties.Resources.resourceCulture;
      }
      set
      {
        OculiService.Common.Properties.Resources.resourceCulture = value;
      }
    }

    internal static string IntrinsicPropertyError
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("IntrinsicPropertyError", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string InvalidEnumFlag
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("InvalidEnumFlag", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string InvalidEnumValue
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("InvalidEnumValue", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string InvalidServer
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("InvalidServer", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string InvalidStateTransition
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("InvalidStateTransition", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string OutOfRange
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("OutOfRange", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string ProvidedStringArgMustNotBeEmpty
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("ProvidedStringArgMustNotBeEmpty", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string SeeInnerException
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("SeeInnerException", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal static string TypesAreNotAssignable
    {
      get
      {
        return OculiService.Common.Properties.Resources.ResourceManager.GetString("TypesAreNotAssignable", OculiService.Common.Properties.Resources.resourceCulture);
      }
    }

    internal Resources()
    {
    }
  }
}
