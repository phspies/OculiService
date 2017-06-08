using OculiService.Common.Properties;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace OculiService.Common
{
  [Serializable]
  public class InvalidStateTransitionException : Exception
  {
    public string Transition { get; private set; }

    public Type CurrentStateType { get; private set; }

    public InvalidStateTransitionException(string transition, Type currentStateType)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidStateTransition, new object[2]{ (object) transition, (object) currentStateType.Name }))
    {
      this.Transition = transition;
      this.CurrentStateType = currentStateType;
    }

    public InvalidStateTransitionException(string transition, Type currentStateType, string message)
      : this(transition, currentStateType, message, (Exception) null)
    {
    }

    public InvalidStateTransitionException(string transition, Type currentStateType, string message, Exception innerException)
      : base(message, innerException)
    {
      this.Transition = transition;
      this.CurrentStateType = currentStateType;
    }

    public InvalidStateTransitionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
