using Common_Util.Properties;
using System;
using System.Globalization;

public class UnitConversionHelper
{
    public static string ConvertBytesToMaxSizeUnit(long size)
    {
        if (size <= 0L)
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.SizeInMB, new object[1] { (object)0 });
        string empty = string.Empty;
        double num = Math.Round((double)size / 1073741824.0, 2);
        string str;
        if (num >= 1.0)
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.SizeInGB, new object[1] { (object)num });
        else
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.SizeInMB, new object[1]
            {
        (object) (double) (size / 1048576L)
            });
        return str;
    }

    public static long ConvertBytesToMBUnit(long size)
    {
        if (size <= 0L)
            return 0;
        return size / 1048576L;
    }

    public static string ConvertMBToMaxSizeUnit(long sizeMB)
    {
        if (sizeMB <= 0L)
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.SizeInMB, new object[1] { (object)0 });
        string empty = string.Empty;
        double num = Math.Round((double)sizeMB / 1024.0, 2);
        string str;
        if (num >= 1.0)
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.SizeInGB, new object[1] { (object)num });
        else
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.SizeInMB, new object[1]
            {
        (object) sizeMB
            });
        return str;
    }

    public static double ConvertMBToMaxSizeUnit(double sizeMB, out string unit)
    {
        unit = "MB";
        if (sizeMB <= 0.0)
            return 0.0;
        double num1 = sizeMB;
        double num2 = Math.Round(sizeMB / 1024.0, 2);
        if (num2 >= 1.0)
        {
            num1 = num2;
            unit = "GB";
        }
        return num1;
    }

    public static long ConvertToBytes(string size)
    {
        if (string.IsNullOrEmpty(size))
            return 0;
        double num = 0.0;
        string[] strArray = size.Split(" ".ToCharArray());
        if (strArray != null)
        {
            if (strArray.Length > 1)
            {
                try
                {
                    num = double.Parse(strArray[0]);
                }
                catch (Exception ex)
                {
                    return 0;
                }
                if (string.Compare(strArray[1], "MB", true, CultureInfo.InvariantCulture) == 0)
                    num *= 1048576.0;
                else if (string.Compare(strArray[1], "GB", true, CultureInfo.InvariantCulture) == 0)
                    num *= 1073741824.0;
            }
        }
        return (long)num;
    }

    public static double ConvertToMB(string size)
    {
        if (string.IsNullOrEmpty(size))
            return 0.0;
        double num = 0.0;
        string[] strArray = size.Split(" ".ToCharArray());
        if (strArray != null)
        {
            if (strArray.Length > 1)
            {
                try
                {
                    num = double.Parse(strArray[0]);
                }
                catch (Exception ex)
                {
                    return 0.0;
                }
                if (string.Compare(strArray[1], "GB", true, CultureInfo.InvariantCulture) == 0)
                    num *= 1024.0;
                else if (string.Compare(strArray[1], "MB", true, CultureInfo.InvariantCulture) != 0)
                    num = 0.0;
            }
        }
        return num;
    }

    public static string ConvertMHertzToMaxUnits(int value)
    {
        if (value <= 0)
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.ValueInMHz, new object[1] { (object)0 });
        string empty = string.Empty;
        string str;
        if (value > 1000)
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.ValueInGHz, new object[1]
            {
        (object) Math.Round((double) value / 1000.0, 2)
            });
        else
            str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resource.ValueInMHz, new object[1]
            {
        (object) value
            });
        return str;
    }

    public static double GetMaxSizeAndUnitFromBytes(long bytes, out string unit)
    {
        unit = "MB";
        if (bytes <= 0L)
            return 0.0;
        double num = Math.Round((double)bytes / 1073741824.0, 2);
        if (num >= 1.0)
            unit = "GB";
        else
            num = (double)(bytes / 1048576L);
        return num;
    }
}
