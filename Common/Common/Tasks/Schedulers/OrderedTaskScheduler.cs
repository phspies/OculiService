namespace OculiService.Common.Tasks.Schedulers
{
  public sealed class OrderedTaskScheduler : LimitedConcurrencyLevelTaskScheduler
  {
    public OrderedTaskScheduler() : base(1)
    {
    }
  }
}
