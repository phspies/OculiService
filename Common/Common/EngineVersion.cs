using System;using System.Collections.Generic;
using System.Linq;

namespace OculiService.Common
{
  public struct EngineVersion : IComparable, IComparable<EngineVersion>, IEquatable<EngineVersion>
  {
    private static EngineVersion zero = new EngineVersion(0, 0, 0, 0, 0);
    private static int[] zeroArray = new int[5];
    private readonly int[] versionNumbers;

    public static EngineVersion Zero
    {
      get
      {
        return EngineVersion.zero;
      }
    }

    public int Major
    {
      get
      {
        return this.VersionNumbers[0];
      }
    }

    public int Minor
    {
      get
      {
        return this.VersionNumbers[1];
      }
    }

    public int ServicePack
    {
      get
      {
        return this.VersionNumbers[2];
      }
    }

    public int SequenceNumber
    {
      get
      {
        return this.VersionNumbers[3];
      }
    }

    public int Hotfix
    {
      get
      {
        return this.VersionNumbers[4];
      }
    }

    private int[] VersionNumbers
    {
      get
      {
        return this.versionNumbers ?? EngineVersion.zeroArray;
      }
    }

    private EngineVersion(int[] numbers)
    {
      Invariant.ArgumentNotNull((object) numbers, "numbers");
      if (numbers.Length != 5)
        throw new ArgumentException();
      this.versionNumbers = numbers;
    }

    public EngineVersion(int major, int minor)
    {
      this = new EngineVersion(major, minor, 0, 0, 0);
    }

    public EngineVersion(int major, int minor, int servicePack)
    {
      this = new EngineVersion(major, minor, servicePack, 0, 0);
    }

    public EngineVersion(int major, int minor, int servicePack, int sequenceNumber)
    {
      this = new EngineVersion(major, minor, servicePack, sequenceNumber, 0);
    }

    public EngineVersion(int major, int minor, int servicePack, int sequenceNumber, int hotfix)
    {
      this = new EngineVersion(new int[5]
      {
        major,
        minor,
        servicePack,
        sequenceNumber,
        hotfix
      });
    }

    public static bool operator ==(EngineVersion a, EngineVersion b)
    {
      return a.CompareTo(b) == 0;
    }

    public static bool operator !=(EngineVersion a, EngineVersion b)
    {
      return (uint) a.CompareTo(b) > 0U;
    }

    public static bool operator >=(EngineVersion a, EngineVersion b)
    {
      return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(EngineVersion a, EngineVersion b)
    {
      return a.CompareTo(b) <= 0;
    }

    public static bool operator >(EngineVersion a, EngineVersion b)
    {
      return a.CompareTo(b) > 0;
    }

    public static bool operator <(EngineVersion a, EngineVersion b)
    {
      return a.CompareTo(b) < 0;
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}.{3}.{4}", (object) this.Major, (object) this.Minor, (object) this.ServicePack, (object) this.SequenceNumber, (object) this.Hotfix);
    }

    public static bool TryParse(string s, out EngineVersion version)
    {
      version = EngineVersion.Zero;
      if (s == null)
        return false;
      string[] strArray = s.Split('.');
      if (strArray.Length != 5)
        return false;
      int[] numbers = new int[5];
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!int.TryParse(strArray[index], out numbers[index]))
          return false;
      }
      version = new EngineVersion(numbers);
      return true;
    }

    public static EngineVersion Parse(string s)
    {
      EngineVersion version;
      if (!EngineVersion.TryParse(s, out version))
        throw new FormatException();
      return version;
    }

    int IComparable.CompareTo(object obj)
    {
      if (obj == null || !(obj is EngineVersion))
        throw new ArgumentException();
      return this.CompareTo((EngineVersion) obj);
    }

    public int CompareTo(EngineVersion other)
    {
      for (int index = 0; index < this.VersionNumbers.Length; ++index)
      {
        int num = Comparer<int>.Default.Compare(this.VersionNumbers[index], other.VersionNumbers[index]);
        if (num != 0)
          return num;
      }
      return 0;
    }

    public bool Equals(EngineVersion other)
    {
      return this.CompareTo(other) == 0;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is EngineVersion))
        return false;
      return this.Equals((EngineVersion) obj);
    }

    public override int GetHashCode()
    {
      return (int) ((IEnumerable<int>) this.VersionNumbers).Aggregate<int, HashCode>(HashCode.From<int>(0), (Func<HashCode, int, HashCode>) ((accum, item) => accum.And<int>(item)));
    }
  }
}
