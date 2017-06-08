namespace OculiService.Common.IO
{
  public class FileSystem : IFileSystem
  {
    private FileBase file;
    private DirectoryBase directory;
    private DriveBase drive;
    private NetworkShareBase networkShare;

    public FileBase File
    {
      get
      {
        return this.file ?? (this.file = (FileBase) new FileAdapter());
      }
    }

    public DirectoryBase Directory
    {
      get
      {
        return this.directory ?? (this.directory = (DirectoryBase) new DirectoryAdapter());
      }
    }

    public DriveBase Drive
    {
      get
      {
        return this.drive ?? (this.drive = (DriveBase) new DriveAdapter());
      }
    }

    public NetworkShareBase NetworkShare
    {
      get
      {
        return this.networkShare ?? (this.networkShare = (NetworkShareBase) new NetworkShareAdapter());
      }
    }
  }
}
