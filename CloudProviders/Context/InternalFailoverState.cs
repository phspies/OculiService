namespace Oculi.Jobs.Context
{
  public enum InternalFailoverState
  {
    NotStarted,
    Started,
    DisconnectedFromOculi,
    Updated,
    DrivesUnmounted,
    DrivesRemoved,
    SourceShutdown,
    FailedOver,
  }
}
