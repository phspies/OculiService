

public interface IPropertySetPrivate
{
    bool Batch { get; set; }

    object GetPrivateProperty(string name);

    void SetReadOnlyProperty(string name, object value);

    void SetPrivateProperty(string name, object value);

    void SetPrivateProperty(string name, object value, PropertyItemAccess access);

    void SetPrivateProperty(string name, object value, PropertyItemAccess access, PropertyItemPermission permission);

    void RegisterPropertyFilter(PropertyFilter filter);

    void UnregisterPropertyFilter(PropertyFilter filter);
}
