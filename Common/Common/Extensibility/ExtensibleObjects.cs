using System;
namespace OculiService.Common.Extensibility
{
  public static class ExtensibleObjects
  {
    public static event EventHandler ApplyingExtensions;

    public static void ApplyExtensions<T>(this IExtensibleObject<T> extensibleObject) where T : class, IExtensibleObject<T>
    {
      
      EventHandler applyingExtensions = ExtensibleObjects.ApplyingExtensions;
      if (applyingExtensions == null)
        return;
      applyingExtensions((object) extensibleObject, EventArgs.Empty);
    }
  }
}
