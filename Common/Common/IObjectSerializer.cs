using System.Runtime.Serialization;

namespace OculiService.Common
{
  public interface IObjectSerializer
  {
    XmlObjectSerializer Serializer { get; }

    string Path { get; }

    object ReadObject();

    void WriteObject(object theObject);
  }
}
