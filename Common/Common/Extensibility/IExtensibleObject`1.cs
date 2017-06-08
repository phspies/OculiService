namespace OculiService.Common.Extensibility
{
  public interface IExtensibleObject<T> where T : class, IExtensibleObject<T>
  {
    IExtensionCollection<T> Extensions { get; }
  }
}
