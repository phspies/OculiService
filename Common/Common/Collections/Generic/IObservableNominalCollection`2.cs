using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common.Collections.Generic
{
  public interface IObservableNominalCollection<TIdentity, TItem> : IObservableCollection<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable
  {
    Func<TItem, TIdentity> IdentitySelector { get; }

    bool TryGetItem(TIdentity identity, out TItem item);

    bool Remove(TIdentity identity);

    void Update(IEnumerable<TItem> items);

    bool UpdateItem(TItem item);
  }
}
