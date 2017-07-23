using System;
using System.Runtime.Serialization;

[Serializable]
public class HyperVException : ApplicationException
{
    public HyperVException()
    {
    }

    public HyperVException(string msg)
      : base(msg)
    {
    }

    public HyperVException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected HyperVException(SerializationInfo info, StreamingContext ctx)
      : base(info, ctx)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
        base.GetObjectData(info, ctx);
    }
}
