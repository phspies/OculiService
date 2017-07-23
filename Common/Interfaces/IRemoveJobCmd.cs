namespace OculiService.Common.Interfaces
{
  public interface IRemoveJobCmd
  {
    void Invoke(bool deleteReplicaVm);
  }
}
