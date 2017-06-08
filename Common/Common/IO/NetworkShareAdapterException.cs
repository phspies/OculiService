using System;using System.Runtime.Serialization;

namespace OculiService.Common.IO
{
  [Serializable]
  public class NetworkShareAdapterException : Exception
  {
    private int _errorCode;

    public int ErrorCode
    {
      get
      {
        return this._errorCode;
      }
    }

    public override string Message
    {
      get
      {
        return string.Format("{0}: {1}", (object) base.Message, (object) this._errorCode);
      }
    }

    public NetworkShareAdapterException()
    {
    }

    public NetworkShareAdapterException(string message, int errorCode = 0)
      : base(message)
    {
      this._errorCode = errorCode;
    }

    public NetworkShareAdapterException(string message, Exception inner, int errorCode = 0)
      : base(message, inner)
    {
      this._errorCode = errorCode;
    }

    protected NetworkShareAdapterException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
