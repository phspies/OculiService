using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OculiService.Common.Extensibility
{
  public interface IExtensionCollection<T> : ICollection<IExtension<T>>, IEnumerable<IExtension<T>>, IEnumerable where T : class, IExtensibleObject<T>
  {
    T this[string name] { get; }

    TExtension Find<TExtension>() where TExtension : T;

    Collection<TExtension> FindAll<TExtension>() where TExtension : T;
  }
}
