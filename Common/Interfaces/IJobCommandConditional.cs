namespace OculiService.Common.Interfaces
{
  public interface IJobCommandConditional
  {
    void Invoke(bool condition);
  }
}
