namespace OculiService.Common.Logging.File.Interface
{
  public interface IArchivingTextWriter
  {
    void Write(LogEntry logEntry);

    void Archive();
  }
}
