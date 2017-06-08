namespace OculiService.Common.IO
{
  public interface IFileSystem
  {
    FileBase File { get; }

    DirectoryBase Directory { get; }

    DriveBase Drive { get; }

    NetworkShareBase NetworkShare { get; }
  }
}
