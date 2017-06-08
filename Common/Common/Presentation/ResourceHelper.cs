using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace OculiService.Common.Presentation
{
  public class ResourceHelper
  {
    private ILogger logger;

    public static CultureInfo CurrentCulture { get; set; }

    public static CultureInfo CurrentUICulture { get; set; }

    public ResourceHelper(ILogger logger)
    {
      this.logger = logger;
    }

    public string GetStringFromKey(Type resourcesType, string resourceKey)
    {
      if (string.IsNullOrEmpty(resourceKey))
        return string.Empty;
      if (resourcesType != (Type) null)
      {
        ResourceManager resourceManager = new ResourceManager(resourcesType);
        resourceManager.IgnoreCase = true;
        string name = resourceKey;
        CultureInfo currentUiCulture = ResourceHelper.CurrentUICulture;
        string str = resourceManager.GetString(name, currentUiCulture);
        if (str != null)
          return str;
      }
      return resourceKey;
    }

    public string GetFormattedStringFromKey(Type resourcesType, string resourceKey, string[] formatArgs)
    {
      string format = this.GetStringFromKey(resourcesType, resourceKey);
      if (string.IsNullOrEmpty(format))
        return string.Empty;
      if (formatArgs != null)
      {
        if (formatArgs.Length != 0)
        {
          try
          {
            format = string.Format(format, (object[]) formatArgs);
          }
          catch (FormatException ex)
          {
            if (this.logger != null)
              this.logger.FormatError("Format error in string '{0}' for key '{1}' with arguments '{2}'.\n{3}", (object) format, (object) resourceKey, (object) string.Join(", ", ((IEnumerable<string>) formatArgs).Select<string, string>((Func<string, string>) (a => "'" + a + "'")).ToArray<string>()), (object) ex);
            format = string.Format(format, (object[]) Enumerable.Repeat<string>("<?>", 20).ToArray<string>());
          }
        }
      }
      return format;
    }

    public string GetStringFromEnumValue(Type resourcesType, object value)
    {
      if (value == null)
        return string.Empty;
      Type type = value.GetType();
      if (type.IsDefined(typeof (FlagsAttribute), false))
        return this.GetFlagsStringFromEnumValue(resourcesType, value);
      return this.GetDescription(resourcesType, type, value.ToString());
    }

    private string GetFlagsStringFromEnumValue(Type resourcesType, object value)
    {
      ulong uint64 = ResourceHelper.ToUInt64(value);
      ulong num = uint64;
      Type type = value.GetType();
      ulong[] array = Enum.GetValues(type).Cast<object>().Select<object, ulong>((Func<object, ulong>) (v => ResourceHelper.ToUInt64(v))).ToArray<ulong>();
      string[] names = Enum.GetNames(type);
      List<string> stringList = new List<string>();
      for (int index = array.Length - 1; index >= 0 && (index != 0 || (long) array[index] != 0L); --index)
      {
        if (((long) uint64 & (long) array[index]) == (long) array[index])
        {
          uint64 -= array[index];
          stringList.Insert(0, this.GetDescription(resourcesType, type, names[index]));
        }
      }
      if ((long) uint64 != 0L)
        return value.ToString();
      if ((long) num != 0L)
        return string.Join(", ", stringList.ToArray());
      if (array.Length != 0 && (long) array[0] == 0L)
        return names[0];
      return "0";
    }

    private string GetDescription(Type resourcesType, Type enumType, string name)
    {
      FieldInfo field = enumType.GetField(name);
      if (field == (FieldInfo) null)
        return string.Empty;
      LocalizedDescriptionAttribute descriptionAttribute = field.GetCustomAttributes(false).OfType<LocalizedDescriptionAttribute>().FirstOrDefault<LocalizedDescriptionAttribute>();
      if (descriptionAttribute != null)
        return descriptionAttribute.Description;
      if (resourcesType != (Type) null)
      {
        ResourceManager resourceManager = new ResourceManager(resourcesType);
        string name1 = enumType.Name + name;
        string str = resourceManager.GetString(name1, ResourceHelper.CurrentUICulture) ?? resourceManager.GetString(name, ResourceHelper.CurrentCulture);
        if (str != null)
          return str;
      }
      return name;
    }

    private static ulong ToUInt64(object value)
    {
      switch (Convert.GetTypeCode(value))
      {
        case TypeCode.Boolean:
        case TypeCode.Char:
        case TypeCode.Byte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          return Convert.ToUInt64(value, (IFormatProvider) CultureInfo.InvariantCulture);
        case TypeCode.SByte:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
          return (ulong) Convert.ToInt64(value, (IFormatProvider) CultureInfo.InvariantCulture);
        default:
          throw new InvalidOperationException("Unknown enum type.");
      }
    }
  }
}
