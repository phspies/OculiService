namespace OculiService.Common
{
  public sealed class LoggerCategory
  {
    public static readonly LoggerCategory Debug = new LoggerCategory("Debug");
    public static readonly LoggerCategory Extensibility = new LoggerCategory("Extensibility");
    public static readonly LoggerCategory General = new LoggerCategory("General");
    public static readonly LoggerCategory Performance = new LoggerCategory("Performance");
    public static readonly LoggerCategory Troubleshooting = new LoggerCategory("Troubleshooting");
    public static readonly LoggerCategory Usability = new LoggerCategory("Usability");
    private readonly string categoryName;

    public string Name
    {
      get
      {
        return this.categoryName;
      }
    }

    private LoggerCategory(string categoryName)
    {
      this.categoryName = categoryName;
    }

    public static LoggerCategory Create(string categoryName)
    {
      return new LoggerCategory(categoryName);
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
