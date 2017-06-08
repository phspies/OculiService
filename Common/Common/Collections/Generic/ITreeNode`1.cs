using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public interface ITreeNode<T> where T : class, ITreeNode<T>
  {
    T Parent { get; }

    IEnumerable<T> Children();
  }
}
