namespace OculiService.CloudProviders.VMware
{
  public struct ScsiSlot
  {
    public int bus;
    public int unit;

    public ScsiSlot(int b, int u)
    {
      this.bus = b;
      this.unit = u;
    }
  }
}
