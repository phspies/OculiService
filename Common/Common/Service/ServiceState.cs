namespace OculiService.Common.Service
{
  public enum ServiceState
  {
    None,
    Stopped,
    StartPending,
    StopPending,
    Running,
    ContinuePending,
    PausePending,
    Paused,
  }
}
