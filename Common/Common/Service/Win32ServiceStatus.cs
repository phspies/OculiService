using System;
namespace OculiService.Common.Service
{
  internal struct Win32ServiceStatus : IEquatable<Win32ServiceStatus>
  {
    public int ServiceType;
    public int CurrentState;
    public int ControlsAccepted;
    public int Win32ExitCode;
    public int ServiceSpecificExitCode;
    public int CheckPoint;
    public int WaitHint;

    public static bool operator ==(Win32ServiceStatus lhs, Win32ServiceStatus rhs)
    {
      return lhs.Equals(rhs);
    }

    public static bool operator !=(Win32ServiceStatus lhs, Win32ServiceStatus rhs)
    {
      return !lhs.Equals(rhs);
    }

    public bool Equals(Win32ServiceStatus other)
    {
      throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
      if (obj is Win32ServiceStatus)
        return this.Equals((Win32ServiceStatus) obj);
      return false;
    }

    public override int GetHashCode()
    {
      HashCode hashCode = HashCode.From<int>(this.ServiceType);
      hashCode = hashCode.And<int>(this.CurrentState);
      hashCode = hashCode.And<int>(this.ControlsAccepted);
      hashCode = hashCode.And<int>(this.Win32ExitCode);
      hashCode = hashCode.And<int>(this.ServiceSpecificExitCode);
      hashCode = hashCode.And<int>(this.CheckPoint);
      return (int) hashCode.And<int>(this.WaitHint);
    }
  }
}
