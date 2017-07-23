using System;
using System.Runtime.Serialization;

[Serializable]
public class NotFailoverableStateException : ApplicationException, ISerializable
{
    public NotFailoverableStateException(string msg)
      : base(msg)
    {
    }
}
