using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

public class SerializerImplFile<T>
{
    private string[] _Name;
    private XmlSerializer xs;
    private string _fileName;
    private string _updateFileName;

    public string[] Name
    {
        get
        {
            return this._Name;
        }
    }

    public string XmlFileName
    {
        get
        {
            return this._fileName;
        }
    }

    public SerializerImplFile(string[] name)
    {
        this._Name = name;
        this.xs = new XmlSerializer(typeof(T), CPropertySet.DataTypes);
        this._fileName = CUtils.BuildFileName(name, ".xml");
        this._updateFileName = this._fileName + "_update";
    }

    public T Load()
    {
        lock (this)
        {
            if (File.Exists(this._fileName))
            {
                try
                {
                    using (Stream resource_0 = (Stream)File.OpenRead(this._fileName))
                        return (T)this.xs.Deserialize(resource_0);
                }
                catch (Exception exception_0)
                {
                }
            }
            if (!File.Exists(this._updateFileName))
                return CUtils.CreateDefault<T>();
            using (Stream resource_1 = (Stream)File.OpenRead(this._updateFileName))
                return (T)this.xs.Deserialize(resource_1);
        }
    }

    public void Save(T item)
    {
        lock (this)
        {
            try
            {
                using (FileStream resource_0 = new FileStream(this._updateFileName, FileMode.Create, FileAccess.Write, FileShare.None, 32768, FileOptions.WriteThrough))
                {
                    this.xs.Serialize((Stream)resource_0, (object)item);
                    resource_0.Flush();
                }
            }
            catch (Exception exception_1)
            {
                throw new ApplicationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Failed to serialize item to \"{0}\" original exception was {1}", new object[2] { (object)this._updateFileName, (object)exception_1.ToString() }));
            }
            try
            {
                using (FileStream resource_1 = new FileStream(this._fileName, FileMode.Create, FileAccess.Write, FileShare.None, 32768, FileOptions.WriteThrough))
                {
                    this.xs.Serialize((Stream)resource_1, (object)item);
                    resource_1.Flush();
                }
            }
            catch (Exception exception_0)
            {
                throw new ApplicationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Failed to serialize item to \"{0}\" original exception was {1}", new object[2] { (object)this._fileName, (object)exception_0.ToString() }));
            }
        }
    }

    public void Delete()
    {
        if (File.Exists(this._fileName))
            File.Delete(this._fileName);
        if (!File.Exists(this._updateFileName))
            return;
        File.Delete(this._updateFileName);
    }
}
