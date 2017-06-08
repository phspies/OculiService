namespace OculiService.Common.Logging.File.Interface
{
  public interface ILogArchiver
  {
    void ArchiveOldFormatLog(string path);

    bool TryArchive(string path);
  }
}
