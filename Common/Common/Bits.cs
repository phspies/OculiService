using System;
namespace OculiService.Common
{
  public static class Bits
  {
    public static bool AreSet(Enum value, Enum flags)
    {
      return Bits.AreSet(Convert.ToInt64((object) value), Convert.ToInt64((object) flags));
    }

    public static bool AreSet(long value, long flags)
    {
      return (value & flags) == flags;
    }

    public static bool AreAnySet(Enum value, Enum flags)
    {
      return Bits.AreAnySet(Convert.ToInt64((object) value), Convert.ToInt64((object) flags));
    }

    public static bool AreAnySet(long value, long flags)
    {
      return (ulong) (value & flags) > 0UL;
    }

    public static T Set<T>(T value, T flags) where T : struct
    {
      return (T) Enum.ToObject(typeof (T), Convert.ToInt64((object) value) | Convert.ToInt64((object) flags));
    }

    public static long Set(long value, long flags)
    {
      return value | flags;
    }

    public static T Clear<T>(T value, T flags) where T : struct
    {
      long num = Convert.ToInt64((object) value) & ~Convert.ToInt64((object) flags);
      return (T) Enum.ToObject(value.GetType(), num);
    }

    public static long Clear(long value, long flags)
    {
      return value &= ~flags;
    }
  }
}
