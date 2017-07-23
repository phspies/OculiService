
public interface IPropertySet
{
    object GetProperty(string name);

    object TryGetProperty(string name);

    void SetProperty(string name, object value);

    void RemoveProperty(string name);

    bool IfDefined(string name);
}
