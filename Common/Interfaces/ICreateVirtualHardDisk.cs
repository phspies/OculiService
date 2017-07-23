namespace OculiService.Common.Interfaces
{
  public interface ICreateVirtualHardDisk
  {
    void Invoke(string filename, long size, string diskType, int sectorSize);
  }
}
