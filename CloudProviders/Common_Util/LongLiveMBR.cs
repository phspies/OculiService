using System;

public class LongLiveMBR : MarshalByRefObject
{
    public override object InitializeLifetimeService()
    {
        return (object)null;
    }
}
