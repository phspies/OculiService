using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

public class CPropertySet : IPropertySet, IPropertySetPrivate
{
    protected Collection<PropertyFilter> Filters = new Collection<PropertyFilter>();
    protected static Type[] _dataTypes;
    protected SortedDictionary<string, PropertyItem> Data;
    protected ObjectSerializer<PropertyItem[]> _serializerFile;
    protected SerializerString _serializerString;
    protected ReaderWriterLock Lock;
    protected string[] Name;
    protected bool _Batch;
    protected bool _Dirty;
    protected bool _deleted;
    protected bool _shutdown;

    public static Type[] DataTypes
    {
        get
        {
            return CPropertySet._dataTypes;
        }
        set
        {
            CPropertySet._dataTypes = value;
        }
    }

    public bool Batch
    {
        get
        {
            return this._Batch;
        }
        set
        {
            this._Batch = value;
            if (value || !this.Dirty || this._deleted)
                return;
            this.Save();
        }
    }

    public PropertyItem[] Items
    {
        get
        {
            return CUtils.CollectionToArray<PropertyItem>((ICollection<PropertyItem>)this.Data.Values);
        }
    }

    public string[] Keys
    {
        get
        {
            return CUtils.CollectionToArray<string>((ICollection<string>)this.Data.Keys);
        }
    }

    public string FileName
    {
        get
        {
            return this._serializerFile.XmlFileName;
        }
    }

    protected bool Dirty
    {
        get
        {
            return this._Dirty;
        }
    }

    protected CPropertySet()
    {
    }

    public CPropertySet(string[] name)
    {
        this._serializerFile = new ObjectSerializer<PropertyItem[]>(name);
        this.Name = name;
        this.Data = new SortedDictionary<string, PropertyItem>();
        this.Lock = new ReaderWriterLock();
        this._serializerString = new SerializerString(CPropertySet._dataTypes);
        this.Load();
    }

    public object GetProperty(string name)
    {
        object obj;
        try
        {
            this.Lock.AcquireReaderLock(-1);
            PropertyItem propertyItem;
            if (this._deleted || !this.Data.TryGetValue(name, out propertyItem))
                throw new PropertyNotExistException("XML property does not exist: \"" + name + "\". XML file name: " + this._serializerFile.XmlFileName);
            this.CheckAccess(propertyItem);
            obj = propertyItem.Value;
            this.InvokePostGetProperty(name, ref obj);
        }
        catch (PropertyNotExistException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new EsxException("Error retrieving XML property: \"" + name + "\". XML file name: " + this._serializerFile.XmlFileName, ex, false);
        }
        finally
        {
            this.Lock.ReleaseReaderLock();
        }
        return obj;
    }

    public bool IfDefined(string name)
    {
        try
        {
            this.Lock.AcquireReaderLock(-1);
            return !this._deleted && this.Data.ContainsKey(name);
        }
        finally
        {
            this.Lock.ReleaseReaderLock();
        }
    }

    public object TryGetProperty(string name)
    {
        try
        {
            return this.GetProperty(name);
        }
        catch (PropertyNotExistException ex)
        {
            return (object)null;
        }
    }

    public void SetProperty(string name, object value)
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (this._deleted)
                return;
            this.InvokePreSetProperty(name, ref value);
            PropertyItem propertyItem;
            if (this.Data.TryGetValue(name, out propertyItem))
            {
                this.CheckPermission(propertyItem);
                if (propertyItem.Value == value)
                    return;
                propertyItem.Value = value;
            }
            else
            {
                propertyItem = new PropertyItem(name, value);
                this.Data[name] = propertyItem;
            }
            this.Save();
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public void RemoveProperty(string name)
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (this._deleted)
                return;
            PropertyItem propertyItem;
            if (this.Data.TryGetValue(name, out propertyItem))
            {
                this.CheckPermission(propertyItem);
                this.Data.Remove(name);
            }
            this.Save();
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public virtual void Load()
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (this._deleted)
                return;
            PropertyItem[] propertyItemArray = this._serializerFile.Load();
            if (propertyItemArray == null)
                return;
            foreach (PropertyItem propertyItem in propertyItemArray)
                CUtils.AddOrReplace<string, PropertyItem>((IDictionary<string, PropertyItem>)this.Data, propertyItem.Name, propertyItem);
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
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
                if (this._deleted || this._shutdown)
                    return;
                PropertyItem[] array = CUtils.CollectionToArray<PropertyItem>((ICollection<PropertyItem>)this.Data.Values);
                try
                {
                    this._serializerFile.Save(array);
                }
                catch (ThreadAbortException ex)
                {
                    this._serializerFile.Save(array);
                }
                this._Dirty = false;
            }
            finally
            {
                this.Lock.ReleaseWriterLock();
            }
        }
    }

    public virtual void Delete()
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (this._deleted)
                return;
            this._serializerFile.Delete();
            this._deleted = true;
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public void Shutdown()
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            this._shutdown = true;
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public object GetPrivateProperty(string name)
    {
        object obj = (object)null;
        try
        {
            this.Lock.AcquireReaderLock(-1);
            if (!this._deleted)
            {
                PropertyItem propertyItem = this.Data[name];
                if (propertyItem != null)
                    obj = propertyItem.Value;
                this.InvokePostGetProperty(name, ref obj);
            }
        }
        finally
        {
            this.Lock.ReleaseReaderLock();
        }
        return obj;
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
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (this._deleted)
                return;
            this.InvokePreSetProperty(name, ref value);
            this.Data[name] = new PropertyItem(name, value, access, permission);
            this.Save();
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public void RegisterPropertyFilter(PropertyFilter filter)
    {
        this.Filters.Add(filter);
    }

    public void UnregisterPropertyFilter(PropertyFilter filter)
    {
        this.Filters.Remove(filter);
    }

    private void CheckAccess(PropertyItem item)
    {
        if (item.Access == PropertyItemAccess.PRIVATE)
            throw new EsxException("Property access denied: " + item.Name, false);
    }

    private void CheckPermission(PropertyItem item)
    {
        if (item.Permission == PropertyItemPermission.READONLY)
            throw new EsxException("Property read only: " + item.Name, false);
    }

    private void InvokePostGetProperty(string name, ref object value)
    {
        foreach (PropertyFilter filter in this.Filters)
            filter.PostGetProperty(name, ref value);
    }

    private void InvokePreSetProperty(string name, ref object value)
    {
        foreach (PropertyFilter filter in this.Filters)
            filter.PreSetProperty(name, ref value);
    }
}
