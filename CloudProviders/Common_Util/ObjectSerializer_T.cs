public class ObjectSerializer<T>
{
  private string[] _Name;
  private SerializerImplFile<T> _Serializer;

  public string XmlFileName
  {
    get
    {
      return this._Serializer.XmlFileName;
    }
  }

  protected string[] Name
  {
    get
    {
      return this._Name;
    }
  }

  protected SerializerImplFile<T> Serializer
  {
    get
    {
      return this._Serializer;
    }
  }

  public ObjectSerializer(string[] name)
  {
    this._Serializer = new SerializerImplFile<T>(name);
    this._Name = name;
  }

  public virtual T Load()
  {
    if (this.Serializer == null)
      return CUtils.CreateDefault<T>();
    return this.Serializer.Load();
  }

  public virtual void Save(T item)
  {
    if (this.Serializer == null)
      return;
    this.Serializer.Save(item);
  }

  public virtual void Delete()
  {
    if (this.Serializer == null)
      return;
    this.Serializer.Delete();
  }
}
