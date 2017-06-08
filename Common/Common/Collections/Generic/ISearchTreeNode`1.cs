using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public interface ISearchTreeNode<T> : IList<ISearchTreeNode<T>>, ICollection<ISearchTreeNode<T>>, IEnumerable<ISearchTreeNode<T>>, IEnumerable
  {
    T Value { get; }

    ISearchTreeNode<T> ParentNode { get; }

    ISearchTreeNode<T> AsReadOnly();
  }
}
