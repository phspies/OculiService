using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public interface IObservableCollection<TItem> : ICollection<TItem>, IEnumerable<TItem>, IEnumerable
  {
    IObservable<CollectionMetadata<TItem>> CollectionItemsAndChanges { get; }

    IObservable<CollectionMetadata<TItem>> CollectionChanges { get; }
  }
}
