namespace OculiService.Common.Interfaces
{
    public interface ICopyDriver
  {
    void Invoke(string driverName, string destinationPath, bool verify);
  }
}
