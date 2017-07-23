public class PropertyItem
{
    public bool IsFromDefault;
    public string Name;
    public PropertyItemAccess Access;
    public PropertyItemPermission Permission;
    private object _Value;
    public object Value
    {
        get
        {
            return this._Value;
        }
        set
        {
            this._Value = value;
            this.IsFromDefault = false;
        }
    }

    public PropertyItem(string name, object value, PropertyItemAccess access, PropertyItemPermission permission)
    {
        this.Name = name;
        this.Value = value;
        this.Access = access;
        this.Permission = permission;
    }

    public PropertyItem(string name, object value, PropertyItemAccess access)
    {
        this.Name = name;
        this.Value = value;
        this.Access = access;
        this.Permission = PropertyItemPermission.READWRITE;
    }

    public PropertyItem(string name, object value)
    {
        this.Name = name;
        this.Value = value;
        this.Access = PropertyItemAccess.PUBLIC;
        this.Permission = PropertyItemPermission.READWRITE;
    }

    public PropertyItem()
    {
        this.Name = (string)null;
        this.Value = (object)null;
        this.Access = PropertyItemAccess.PUBLIC;
        this.Permission = PropertyItemPermission.READWRITE;
    }
}
