namespace OculiService.Common.Interfaces
{
    public interface IBuildDiskName : ITaskCommandBase
    {
        string Invoke(string serverName, string diskName);
    }
}
