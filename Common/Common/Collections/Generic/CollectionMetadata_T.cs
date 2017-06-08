using System.Runtime.Serialization;

namespace OculiService.Common.Collections.Generic
{
  public class CollectionMetadata<T>
  {
    public CollectionMetadataAction Action { get; set; }
    public T Item { get; set; }
  }
}
