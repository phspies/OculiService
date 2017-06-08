using System;using System.Globalization;
using System.Text;

namespace OculiService.Common
{
  public class Range<T> : IFormattable where T : IComparable
  {
    private readonly T _lowerBound;
    private readonly RangeBoundaryType _lowerBoundaryType;
    private readonly T _upperBound;
    private readonly RangeBoundaryType _upperBoundaryType;

    private bool IsValid
    {
      get
      {
        if (this._lowerBoundaryType == RangeBoundaryType.Ignore && this._upperBoundaryType == RangeBoundaryType.Ignore)
          return false;
        if (this._lowerBoundaryType == RangeBoundaryType.Ignore || this._upperBoundaryType == RangeBoundaryType.Ignore)
          return true;
        int num1 = this._lowerBound.CompareTo((object) this._upperBound);
        if (num1 > 0 || num1 == 0 && this._lowerBoundaryType == RangeBoundaryType.Exclusive)
          return false;
        int num2 = this._upperBound.CompareTo((object) this._lowerBound);
        return num2 >= 0 && (num2 != 0 || this._upperBoundaryType != RangeBoundaryType.Exclusive);
      }
    }

    public T LowerBound
    {
      get
      {
        return this._lowerBound;
      }
    }

    public RangeBoundaryType LowerBoundaryType
    {
      get
      {
        return this._lowerBoundaryType;
      }
    }

    public T UpperBound
    {
      get
      {
        return this._upperBound;
      }
    }

    public RangeBoundaryType UpperBoundaryType
    {
      get
      {
        return this._upperBoundaryType;
      }
    }

    public Range(T lowerBound, T upperBound)
      : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
    {
    }

    public Range(T lowerBound, RangeBoundaryType lowerBoundaryType, T upperBound, RangeBoundaryType upperBoundaryType)
    {
      Invariant.ArgumentEnumIsValidValue((Enum) lowerBoundaryType, "lowerBoundaryType");
      Invariant.ArgumentEnumIsValidValue((Enum) upperBoundaryType, "upperBoundaryType");
      this._lowerBound = lowerBound;
      this._lowerBoundaryType = lowerBoundaryType;
      this._upperBound = upperBound;
      this._upperBoundaryType = upperBoundaryType;
      if (!this.IsValid)
        throw new InvalidOperationException();
    }

    public bool Contains(T value)
    {
      if (this._lowerBoundaryType != RangeBoundaryType.Ignore)
      {
        int num = this._lowerBound.CompareTo((object) value);
        if (num > 0 || num == 0 && this._lowerBoundaryType == RangeBoundaryType.Exclusive)
          return false;
      }
      if (this._upperBoundaryType != RangeBoundaryType.Ignore)
      {
        int num = this._upperBound.CompareTo((object) value);
        if (num < 0 || num == 0 && this._upperBoundaryType == RangeBoundaryType.Exclusive)
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      return this.ToString("G", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string ToString(IFormatProvider formatProvider)
    {
      return this.ToString("G", formatProvider);
    }

    public string ToString(string format)
    {
      return this.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      if (formatProvider != null)
      {
        ICustomFormatter format1 = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
        if (format1 != null)
          return format1.Format(format, (object) this, formatProvider);
      }
      if (string.IsNullOrEmpty(format))
        format = "G";
      if (!(format == "G"))
        throw new FormatException();
      StringBuilder stringBuilder = new StringBuilder();
      if (this._lowerBoundaryType == RangeBoundaryType.Ignore)
        stringBuilder.Append("[?-");
      else
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}-", new object[2]
        {
          (object) (this._lowerBoundaryType == RangeBoundaryType.Inclusive ? "[" : "("),
          (object) this._lowerBound
        });
      if (this._upperBoundaryType == RangeBoundaryType.Ignore)
        stringBuilder.Append("?]");
      else
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", new object[2]
        {
          (object) this._upperBound,
          (object) (this._upperBoundaryType == RangeBoundaryType.Inclusive ? "]" : ")")
        });
      return stringBuilder.ToString();
    }
  }
}
