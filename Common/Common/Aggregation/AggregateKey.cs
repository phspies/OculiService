using System;
using System.Text;

namespace OculiService.Common.Aggregation
{
  public sealed class AggregateKey : IEquatable<AggregateKey>
  {
    private readonly Type dataType;
    private readonly string name;

    public Type DataType
    {
      get
      {
        return this.dataType;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public AggregateKey(Type dataType, string name)
    {
      this.dataType = dataType;
      this.name = name ?? string.Empty;
    }

    public override int GetHashCode()
    {
      return (int) HashCode.From<Type>(this.dataType).And<string>(this.name);
    }

    public override bool Equals(object obj)
    {
      AggregateKey other = obj as AggregateKey;
      if (other == null)
        return false;
      return this.Equals(other);
    }

    public bool Equals(AggregateKey other)
    {
      if (this.dataType == other.dataType)
        return this.name == other.name;
      return false;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("AggregateKey { ");
      if (!string.IsNullOrEmpty(this.name))
        stringBuilder.AppendFormat("Name = '{0}', ", (object) this.name);
      stringBuilder.AppendFormat("Type = '{0}' }", (object) this.dataType);
      return stringBuilder.ToString();
    }
  }
}
