using System;
using System.Globalization;
using System.Runtime.Serialization;

[Serializable]
public class HyperVElementNotFoundException : HyperVException
{
    public HyperVElementNotFoundException(string element)
      : base(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Element {0} is not found", new object[1] { (object)element }))
    {
    }

    protected HyperVElementNotFoundException(SerializationInfo info, StreamingContext ctx)
      : base(info, ctx)
    {
    }
}
