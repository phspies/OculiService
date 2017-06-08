using System;using System.Collections;
using System.Collections.Generic;

namespace OculiService.Common
{
  public class NamedArgumentCollection : IEnumerable<KeyValuePair<string, string>>, IEnumerable
  {
    private IDictionary<string, string> _collection;

    public string this[string key]
    {
      get
      {
        return this._collection[key];
      }
    }

    public NamedArgumentCollection()
    {
      this._collection = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ParseArgs((IEnumerable<string>) Environment.GetCommandLineArgs());
    }

    public NamedArgumentCollection(IEnumerable<string> args)
    {
      this._collection = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ParseArgs(args);
    }

    protected void ParseArgs(IEnumerable<string> args)
    {
      foreach (string str in args)
      {
        if (str.StartsWith("/", StringComparison.Ordinal) || str.StartsWith("-", StringComparison.Ordinal))
        {
          string key = str.TrimStart('/', '-');
          int length = key.IndexOf(':');
          if (length == -1)
            this._collection.Add(key, string.Empty);
          else
            this._collection.Add(key.Substring(0, length), key.Substring(length + 1).Trim('"'));
        }
      }
    }

    public bool Contains(string name)
    {
      return this._collection.ContainsKey(name);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return this._collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
