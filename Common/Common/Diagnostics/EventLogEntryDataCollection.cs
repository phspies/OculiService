using System;using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OculiService.Common.Diagnostics
{
  internal class EventLogEntryDataCollection : IList<EventLogEntryData>, ICollection<EventLogEntryData>, IEnumerable<EventLogEntryData>, IEnumerable
  {
    private EventLogEntryCollection collection;

    public int Count
    {
      get
      {
        return this.collection.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public EventLogEntryData this[int index]
    {
      get
      {
        return (EventLogEntryData) new EventLogEntryWrapper(this.collection[index]);
      }
      set
      {
        throw new InvalidOperationException("This collection is read-only");
      }
    }

    public EventLogEntryDataCollection(EventLogEntryCollection collection)
    {
      this.collection = collection;
    }

    public void Add(EventLogEntryData item)
    {
      throw new InvalidOperationException("This collection is read-only");
    }

    public void Clear()
    {
      throw new InvalidOperationException("This collection is read-only");
    }

    public bool Contains(EventLogEntryData item)
    {
      return this.collection.Cast<EventLogEntryData>().Contains<EventLogEntryData>(item);
    }

    public void CopyTo(EventLogEntryData[] array, int arrayIndex)
    {
      EventLogEntryData[] array1 = this.ToArray<EventLogEntryData>();
      Array.Copy((Array) array1, 0, (Array) array, arrayIndex, array1.Length);
    }

    public bool Remove(EventLogEntryData item)
    {
      throw new InvalidOperationException("This collection is read-only");
    }

    public IEnumerator<EventLogEntryData> GetEnumerator()
    {
      return this.collection.Cast<EventLogEntry>().Select<EventLogEntry, EventLogEntryData>((Func<EventLogEntry, EventLogEntryData>) (entry => (EventLogEntryData) new EventLogEntryWrapper(entry))).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public int IndexOf(EventLogEntryData item)
    {
      throw new NotImplementedException();
    }

    public void Insert(int index, EventLogEntryData item)
    {
      throw new InvalidOperationException("This collection is read-only");
    }

    public void RemoveAt(int index)
    {
      throw new InvalidOperationException("This collection is read-only");
    }
  }
}
