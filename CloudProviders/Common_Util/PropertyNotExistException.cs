using System.Runtime.Serialization;

public class PropertyNotExistException : EsxException, ISerializable
{
  public PropertyNotExistException(string name)
    : base(name, false)
  {
  }

  protected PropertyNotExistException(SerializationInfo info, StreamingContext ctx)
    : base(info, ctx)
  {
  }
}
