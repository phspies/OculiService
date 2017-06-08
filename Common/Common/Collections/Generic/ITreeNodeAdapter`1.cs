using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public interface ITreeNodeAdapter<T> where T : class
  {
    T GetParent(T node);

    IEnumerable<T> GetChildren(T node);
  }
}
