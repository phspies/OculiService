namespace OculiService.ComponentModel
{
  public class PropertyChangedEventInfo
  {
    private readonly string propertyName;
    private readonly object sender;

    public string PropertyName
    {
      get
      {
        return this.propertyName;
      }
    }

    public object Sender
    {
      get
      {
        return this.sender;
      }
    }

    public PropertyChangedEventInfo(object sender, string propertyName)
    {
      this.sender = sender;
      this.propertyName = propertyName;
    }
  }
}
