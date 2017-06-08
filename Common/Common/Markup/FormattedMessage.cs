using System;using System.Globalization;
using System.Linq;
using System.Resources;

namespace OculiService.Common.Markup
{
  public class FormattedMessage
  {
    private static CultureInfo neutral = CultureInfo.GetCultureInfo("en-US");
    private ResourceManager manager;
    private object[] parameters;
    private string resourceKey;

    private FormattedMessage(Type resourceType, string resourceKey, params object[] parameters)
    {
      Invariant.ArgumentNotNull((object) resourceType, "resourceType");
      Invariant.ArgumentNotNullOrEmpty(resourceKey, "resourceKey");
      Invariant.ArgumentNotNull((object) parameters, "parameters");
      if (parameters.OfType<FormattedMessage>().Any<FormattedMessage>())
        throw new ArgumentOutOfRangeException("FormattedMessage instances are not supported in parameters list", "parameters");
      this.resourceKey = resourceKey;
      this.parameters = parameters;
      this.manager = new ResourceManager(resourceType);
    }

    public static FormattedMessage From<TResource>(string resourceKey, params object[] parameters)
    {
      return new FormattedMessage(typeof (TResource), resourceKey, parameters);
    }

    public override string ToString()
    {
      return this.ToString((CultureInfo) null);
    }

    public string ToString(CultureInfo culture)
    {
      CultureInfo cultureInfo = CultureInfo.CurrentCulture;
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      else if (culture == FormattedMessage.neutral)
        cultureInfo = FormattedMessage.neutral;
      string format = this.manager.GetString(this.resourceKey, culture);
      if (format == null && culture != FormattedMessage.neutral)
      {
        culture = cultureInfo = FormattedMessage.neutral;
        format = this.manager.GetString(this.resourceKey, culture);
      }
      if (format == null)
        return string.Empty;
      if (this.parameters.Length == 0)
        return format;
      return string.Format((IFormatProvider) cultureInfo, format, this.parameters);
    }

    public string ToStringNeutral()
    {
      return this.ToString(FormattedMessage.neutral);
    }
  }
}
