namespace OculiService.CloudProviders.Contract
{
  public static class VirtualSwitchMappingExtensions
  {
    public static bool IsConnected(this VirtualSwitchMapping mapping)
    {
      return !string.IsNullOrEmpty(mapping.TargetVirtualSwitch.Label);
    }
    public static bool IsDiscarded(this VirtualSwitchMapping mapping)
    {
      return "---Discard---" == mapping.TargetVirtualSwitch.Label;
    }
  }
}
