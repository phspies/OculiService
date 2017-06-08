using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OculiService.Common.Collections.Generic
{
  internal class CollectionMetadataGenerator<TItem>
  {
    private readonly IScheduler scheduler;
    private readonly Subject<CollectionMetadata<TItem>> output;

    public CollectionMetadataGenerator(IScheduler scheduler)
    {
      this.scheduler = scheduler;
      this.output = new Subject<CollectionMetadata<TItem>>();
    }

    public IObservable<CollectionMetadata<TItem>> CollectionItemsAndChanges(Func<IEnumerable<TItem>> getItems)
    {
      return Observable.Defer<CollectionMetadata<TItem>>((Func<IObservable<CollectionMetadata<TItem>>>) (() => ((IEnumerable<CollectionMetadata<TItem>>) getItems().Select<TItem, CollectionMetadata<TItem>>((Func<TItem, CollectionMetadata<TItem>>) (item => new CollectionMetadata<TItem>() { Action = CollectionMetadataAction.Replay, Item = item })).Concat<CollectionMetadata<TItem>>(new CollectionMetadata<TItem>() { Action = CollectionMetadataAction.ReplayComplete }).ToArray<CollectionMetadata<TItem>>()).ToObservable<CollectionMetadata<TItem>>(this.scheduler).Concat<CollectionMetadata<TItem>>(this.CollectionChanges())));
    }

    public IObservable<CollectionMetadata<TItem>> CollectionChanges()
    {
      return this.output.AsObservable<CollectionMetadata<TItem>>();
    }

    public void OnUpdated(TItem item)
    {
      this.output.OnNext(new CollectionMetadata<TItem>()
      {
        Action = CollectionMetadataAction.Update,
        Item = item
      });
    }

    public void OnAdded(TItem item)
    {
      this.output.OnNext(new CollectionMetadata<TItem>()
      {
        Action = CollectionMetadataAction.Add,
        Item = item
      });
    }

    public void OnRemoved(TItem item)
    {
      this.output.OnNext(new CollectionMetadata<TItem>()
      {
        Action = CollectionMetadataAction.Remove,
        Item = item
      });
    }

    public void OnCompleted()
    {
      this.output.OnCompleted();
    }
  }
}
