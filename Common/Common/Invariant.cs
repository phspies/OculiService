using OculiService.Common.Properties;
using System;
using System.Diagnostics;
using System.Globalization;

namespace OculiService.Common
{
  public static class Invariant
  {
    [DebuggerHidden]
    public static void ArgumentAssert(bool condition, string argumentName, string message)
    {
      if (!condition)
        throw new ArgumentException(message, argumentName);
    }

    [DebuggerHidden]
    public static void ArgumentEnumDoesNotIncludeFlags(Enum value, Enum flags, string argumentName)
    {
      if (Bits.AreAnySet(value, flags))
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidEnumFlag, new object[2]{ (object) value, (object) value.GetType() });
        throw new ArgumentOutOfRangeException(argumentName, (object) value, message);
      }
    }

    [DebuggerHidden]
    public static void ArgumentEnumIsValidValue(Enum value, string argumentName)
    {
      Invariant.ArgumentNotNull((object) value, argumentName);
      bool flag = true;
      if (value.GetType().GetCustomAttributes(typeof (FlagsAttribute), false).Length != 0)
      {
        if (!Invariant.AreFlagsValid(value))
          flag = false;
      }
      else if (!Enum.IsDefined(value.GetType(), (object) value))
        flag = false;
      if (!flag)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidEnumValue, new object[2]{ (object) value, (object) value.GetType() });
        throw new ArgumentOutOfRangeException(argumentName, (object) value, message);
      }
    }

    [DebuggerHidden]
    public static void ArgumentIsInRange<T>(T value, string argumentName, Range<T> range) where T : IComparable
    {
      if (!range.Contains(value))
        throw new ArgumentOutOfRangeException(argumentName, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.OutOfRange, new object[2]{ (object) value, (object) range }));
    }

    [DebuggerHidden]
    public static void ArgumentNotNull(object argumentValue, string argumentName)
    {
      if (argumentValue == null)
        throw new ArgumentNullException(argumentName);
    }

    [DebuggerHidden]
    public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
    {
      if (argumentValue == null)
        throw new ArgumentNullException(argumentName);
      if (argumentValue.Length == 0)
        throw new ArgumentException(Resources.ProvidedStringArgMustNotBeEmpty, argumentName);
    }

    [DebuggerHidden]
    public static void ArgumentTypeIsAssignable(Type targetType, Type sourceType, string argumentName)
    {
      Invariant.ArgumentNotNull((object) targetType, "targetType");
      Invariant.ArgumentNotNull((object) sourceType, "sourceType");
      if (!targetType.IsAssignableFrom(sourceType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TypesAreNotAssignable, new object[2]{ (object) targetType, (object) sourceType }), argumentName);
    }

    public static void ArgumentValidTimeout(int millisecondsTimeout, string argumentName)
    {
      if (millisecondsTimeout < 0 && millisecondsTimeout != -1)
        throw new ArgumentOutOfRangeException(argumentName);
    }

    public static void ArgumentValidTimeout(TimeSpan timeout, string argumentName)
    {
      if (timeout.TotalMilliseconds < 0.0 && timeout.TotalMilliseconds != -1.0 || timeout.TotalMilliseconds > (double) int.MaxValue)
        throw new ArgumentOutOfRangeException(argumentName);
    }

    private static bool AreFlagsValid(Enum value)
    {
      if (Convert.ToInt64((object) value, (IFormatProvider) CultureInfo.InvariantCulture) < 0L)
        return false;
      ulong uint64_1 = Convert.ToUInt64((object) value, (IFormatProvider) CultureInfo.InvariantCulture);
      Array values = Enum.GetValues(value.GetType());
      int index = values.Length - 1;
      for (; index >= 0; --index)
      {
        ulong uint64_2 = Convert.ToUInt64(values.GetValue(index), (IFormatProvider) CultureInfo.InvariantCulture);
        if (((long) uint64_1 & (long) uint64_2) == (long) uint64_2)
        {
          uint64_1 -= uint64_2;
          if ((long) uint64_1 == 0L)
            return true;
        }
      }
      return false;
    }
  }
}
