using System;
namespace OculiService.Common.Extensibility
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ExtensionNameAttribute : Attribute
  {
    private readonly string name;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public ExtensionNameAttribute(string name)
    {
      this.name = name;
    }
  }
}
