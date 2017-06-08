namespace OculiService.ComponentModel
{
  public sealed class PropertyChangedEventInfo<T> : PropertyChangedEventInfo
  {
    private readonly T newValue;
    private readonly T oldValue;

    public T NewValue
    {
      get
      {
        return this.newValue;
      }
    }

    public T OldValue
    {
      get
      {
        return this.oldValue;
      }
    }

    public PropertyChangedEventInfo(object sender, string propertyName, T oldValue, T newValue)
      : base(sender, propertyName)
    {
      this.oldValue = oldValue;
      this.newValue = newValue;
    }
  }
}
