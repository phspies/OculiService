namespace OculiService.Common.Interfaces
{
  public interface IFormatDrive
  {
    void Invoke(string volumeId, bool quickFormat, string format, string label, long diskSize);
  }
}
