using System;

public class EsxDeviceBusyException : Exception
{
    public EsxDeviceBusyException()
    {
    }

    public EsxDeviceBusyException(string msg)
      : base(msg)
    {
    }
}
