using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

public class AsPropertySetAdapter<T> : IPropertySet, IPropertySetPrivate
{
    private DataContractSerializer _Serializer;
    private T _Ref;
    public string SerializedData;
    private string _FileName;
    private string _UpdateFileName;
    private bool _Batch;
    private bool _Dirty;
    private ReaderWriterLock Lock;
    private string[] Name;

    public bool Batch
    {
        get
        {
            return this._Batch;
        }
        set
        {
            this._Batch = value;
            if (value || !this.Dirty)
                return;
            this.Save();
        }
    }

    public bool Dirty
    {
        get
        {
            return this._Dirty;
        }
    }

    public AsPropertySetAdapter(T o, string[] name)
    {
        this.Name = name;
        this._Ref = o;
        this._Serializer = new DataContractSerializer(typeof(T));
        this.Lock = new ReaderWriterLock();
        this._FileName = CUtils.BuildFileName(this.Name, ".xml");
        this._UpdateFileName = this._FileName + "_update";
    }

    public object GetProperty(string name)
    {
        object property = this.TryGetProperty(name);
        if (property != null)
            return property;
        throw new PropertyNotExistException("The property \"" + name + "\" does not exist in " + this.GetType().Name);
    }

    public object TryGetProperty(string name)
    {
        FieldInfo field = this._Ref.GetType().GetField(name);
        if (!(field != (FieldInfo)null))
            return (object)null;
        this.VerifyAccess(field, PropertyItemAccess.PUBLIC);
        return field.GetValue((object)this._Ref);
    }

    public void SetProperty(string name, object value)
    {
        this.SetPrivateProperty(name, value, PropertyItemAccess.PUBLIC);
    }

    public void RemoveProperty(string name)
    {
        throw new NotImplementedException();
    }

    public bool IfDefined(string name)
    {
        return this._Ref.GetType().GetField(name) != (FieldInfo)null;
    }

    public object GetPrivateProperty(string name)
    {
        FieldInfo field = this._Ref.GetType().GetField(name);
        if (field == (FieldInfo)null)
            throw new PropertyNotExistException("The property \"" + name + "\" does not exist in " + this.GetType().Name);
        this.VerifyAccess(field, PropertyItemAccess.PRIVATE);
        return field.GetValue((object)this._Ref);
    }

    public void SetReadOnlyProperty(string name, object value)
    {
        this.SetPrivateProperty(name, value, PropertyItemAccess.PUBLIC, PropertyItemPermission.READONLY);
    }

    public void SetPrivateProperty(string name, object value)
    {
        this.SetPrivateProperty(name, value, PropertyItemAccess.PRIVATE);
    }

    public void SetPrivateProperty(string name, object value, PropertyItemAccess access)
    {
        this.SetPrivateProperty(name, value, access, PropertyItemPermission.READWRITE);
    }

    public void SetPrivateProperty(string name, object value, PropertyItemAccess access, PropertyItemPermission permission)
    {
        FieldInfo field = this._Ref.GetType().GetField(name);
        if (field == (FieldInfo)null)
            throw new PropertyNotExistException("The property \"" + name + "\" does not exist in " + this.GetType().Name);
        this.VerifyAccess(field, access);
        this.VerifyPermission(field, permission);
        this.SetValue(field, (object)this._Ref, value);
    }

    public void RegisterPropertyFilter(PropertyFilter filter)
    {
        throw new NotImplementedException();
    }

    public void UnregisterPropertyFilter(PropertyFilter filter)
    {
        throw new NotImplementedException();
    }

    public virtual void Save()
    {
        if (this.Batch)
        {
            this._Dirty = true;
        }
        else
        {
            try
            {
                this.Lock.AcquireWriterLock(-1);
                using (FileStream fileStream = new FileStream(this._UpdateFileName, FileMode.Create, FileAccess.Write, FileShare.None, 32768, FileOptions.WriteThrough))
                    this._Serializer.WriteObject((Stream)fileStream, (object)this._Ref);
                using (FileStream fileStream = new FileStream(this._FileName, FileMode.Create, FileAccess.Write, FileShare.None, 32768, FileOptions.WriteThrough))
                    this._Serializer.WriteObject((Stream)fileStream, (object)this._Ref);
                this._Dirty = false;
            }
            finally
            {
                this.Lock.ReleaseWriterLock();
            }
        }
    }

    public virtual void Load()
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (File.Exists(this._FileName))
            {
                try
                {
                    using (FileStream fileStream = new FileStream(this._FileName, FileMode.Open, FileAccess.Read, FileShare.None, 32768, FileOptions.WriteThrough))
                    {
                        object obj = this._Serializer.ReadObject((Stream)fileStream);
                        foreach (FieldInfo field in obj.GetType().GetFields())
                            field.SetValue((object)this._Ref, field.GetValue(obj));
                        return;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (File.Exists(this._UpdateFileName))
            {
                try
                {
                    using (Stream stream = (Stream)File.OpenRead(this._UpdateFileName))
                    {
                        object obj = this._Serializer.ReadObject(stream);
                        foreach (FieldInfo field in obj.GetType().GetFields())
                            field.SetValue((object)this._Ref, field.GetValue(obj));
                        return;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            this.Save();
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public virtual void Delete()
    {
        if (File.Exists(this._FileName))
            File.Delete(this._FileName);
        if (!File.Exists(this._UpdateFileName))
            return;
        File.Delete(this._UpdateFileName);
    }

    internal void SetValue(FieldInfo fi, object target, object newValue)
    {
        if (newValue is IComparable)
        {
            object obj = fi.GetValue(target);
            if (((IComparable)newValue).CompareTo(obj) == 0)
                return;
            fi.SetValue(target, newValue);
            this.Save();
        }
        else
        {
            fi.SetValue(target, newValue);
            this.Save();
        }
    }

    internal PropertyItemPermission GetPermission(FieldInfo fi)
    {
        foreach (object customAttribute in fi.GetCustomAttributes(false))
        {
            if (customAttribute is ReadOnlyPropertyAttribute)
                return PropertyItemPermission.READONLY;
        }
        return PropertyItemPermission.READWRITE;
    }

    internal void VerifyPermission(FieldInfo fi, PropertyItemPermission p)
    {
        if (this.GetPermission(fi) != p)
            throw new ApplicationException("Item " + fi.Name + " is not a " + (object)p + " property");
    }

    internal PropertyItemAccess GetAccess(FieldInfo fi)
    {
        foreach (object customAttribute in fi.GetCustomAttributes(false))
        {
            if (customAttribute is PrivatePropertyAttribute)
                return PropertyItemAccess.PRIVATE;
        }
        return PropertyItemAccess.PUBLIC;
    }

    internal void VerifyAccess(FieldInfo fi, PropertyItemAccess a)
    {
        if (this.GetAccess(fi) != a)
            throw new ApplicationException("Item " + fi.Name + " is not a " + (object)a + "property");
    }
}
