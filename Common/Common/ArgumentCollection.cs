using System;
using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common
{
  public class ArgumentCollection : IEnumerable<string>, IEnumerable
  {
    private IList<string> _collection;
    private NamedArgumentCollection _named;
    private UnnamedArgumentCollection _unnamed;

    public string this[int index]
    {
      get
      {
        return this._collection[index];
      }
    }

    public NamedArgumentCollection Named
    {
      get
      {
        return this._named;
      }
    }

    public UnnamedArgumentCollection Unnamed
    {
      get
      {
        return this._unnamed;
      }
    }

    public ArgumentCollection()
      : this((IEnumerable<string>) Environment.GetCommandLineArgs())
    {
    }

    public ArgumentCollection(IEnumerable<string> args)
    {
      this._collection = (IList<string>) new List<string>(args);
      this._named = new NamedArgumentCollection(args);
      this._unnamed = new UnnamedArgumentCollection(args);
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
