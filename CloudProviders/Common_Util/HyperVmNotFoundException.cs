using System;
using System.Runtime.Serialization;

[Serializable]
public class HyperVmNotFoundException : HyperVException
{
    public HyperVmNotFoundException(string vm)
      : base(vm)
    {
    }

    protected HyperVmNotFoundException(SerializationInfo info, StreamingContext ctx)
      : base(info, ctx)
    {
    }
}
