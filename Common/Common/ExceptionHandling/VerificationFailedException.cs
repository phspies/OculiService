using System;using System.Runtime.Serialization;

namespace OculiService.Common.ExceptionHandling
{
  [Serializable]
  public class VerificationFailedException : Exception
  {
    public VerificationFailedException()
      : this("")
    {
    }

    public VerificationFailedException(string message)
      : this(message, (Exception) null)
    {
    }

    public VerificationFailedException(string message, Exception inner)
      : base("Validation encountered an unhandled exception: " + message, inner)
    {
    }

    protected VerificationFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
