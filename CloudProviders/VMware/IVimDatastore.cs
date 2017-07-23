using OculiService.CloudProviders.Contract;
using System.Collections.Generic;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimDatastore : IVimManagedItem
  {
    long Capacity { get; }

    long FreeSpace { get; }

    DatastoreProperties DsProperties { get; }

    string BracketedName { get; }

    DatastoreProperties GetCommonProperties();

    void GetCommonProperties(Dictionary<string, object> properties);

    string GetPath();

    IVimDatacenter GetDatacenterAndProperties();

    IVimHost[] GetHosts();

    Dictionary<string, List<VmdkFileInfo>> GetAllFoldersAndFilesInfo(VimClientlContext ctx);

    Dictionary<string, VmdkFileInfo> GetVmdksFileInfo(string folderName, VimClientlContext ctx);

    Dictionary<string, VimDatastoreItem[]> FindDatastoreItems(VimClientlContext ctx);

    VimDatastoreItem[] FindDatastoreItemsInFolder(string folderName, VimClientlContext ctx);

    string[] GetVmdksFullName(string folderName, VimClientlContext ctx);

    bool IsFolderOnRootExist(string folderName, VimClientlContext ctx);

    void DeleteFile(string filePath, VimClientlContext ctx);

    Dictionary<string, bool> GetVirtualDisksTypes(string folderName, VimClientlContext ctx);

    void MoveFilesByFullName(string source, string target, string targetFolderName, bool force, VimClientlContext ctx);

    void MoveFilesByName(string srcFolder, string srcFile, string tgtFolder, bool force, VimClientlContext ctx);

    void DownloadFile(string remotePath, string localPath, string fileName, VimClientlContext ctx);

    void UploadFile(string remotePath, string localPath, string fileName);

    bool DirectoryExist(string dir, VimClientlContext ctx);

    bool FileExist(string fullName, VimClientlContext ctx);

    void CreateDirectory(string folder);

    bool IsReadOnly(IVimHost vimHost);
  }
}
