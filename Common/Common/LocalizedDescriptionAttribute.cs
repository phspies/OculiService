using System;using System.Globalization;
using System.Resources;

namespace OculiService.Common
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
  public sealed class LocalizedDescriptionAttribute : Attribute
  {
    private string resourceName;
    private Type resourceType;

    public string ResourceName
    {
      get
      {
        return this.resourceName;
      }
      set
      {
        this.resourceName = value;
      }
    }

    public Type ResourceType
    {
      get
      {
        return this.resourceType;
      }
      set
      {
        this.resourceType = value;
      }
    }

    public string Description
    {
      get
      {
        if (this.resourceName == null || this.resourceType == (Type) null)
          return (string) null;
        return LocalizedDescriptionAttribute.GetString(this.resourceName, this.resourceType);
      }
    }

    private static string GetString(string resourceName, Type resourceType)
    {
      return new ResourceManager(resourceType).GetString(resourceName, CultureInfo.CurrentUICulture);
    }
  }
}
