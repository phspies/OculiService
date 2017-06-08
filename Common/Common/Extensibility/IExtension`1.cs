namespace OculiService.Common.Extensibility
{
  public interface IExtension<T> where T : class, IExtensibleObject<T>
  {
    void Attach(T owner);

    void Detach(T owner);
  }
}
