using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common
{
  public class UnnamedArgumentCollection : IEnumerable<string>, IEnumerable
  {
    private IList<string> _collection;

    public int Count
    {
      get
      {
        return this._collection.Count;
      }
    }

    public string this[int index]
    {
      get
      {
        return this._collection[index];
      }
    }

    public UnnamedArgumentCollection()
    {
      this._collection = (IList<string>) new List<string>();
      this.ParseArgs((IEnumerable<string>) Environment.GetCommandLineArgs());
    }

    public UnnamedArgumentCollection(IEnumerable<string> args)
    {
      this._collection = (IList<string>) new List<string>();
      this.ParseArgs(args);
    }

    protected void ParseArgs(IEnumerable<string> args)
    {
      foreach (string str in args)
      {
        if (!str.StartsWith("/", StringComparison.Ordinal))
          this._collection.Add(str);
      }
    }

    public IEnumerator<string> GetEnumerator()
    {
      return this._collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
