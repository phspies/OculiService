
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

public class SerializerString
{
    private List<Type> _knownTypeList = new List<Type>();
    private DataContractSerializer _dcs;

    public SerializerString(Type[] dataTypes)
    {
        foreach (Type dataType in dataTypes)
            this._knownTypeList.Add(dataType);
        this._dcs = new DataContractSerializer(typeof(PropertyItem[]), (IEnumerable<Type>)this._knownTypeList);
    }

    public string ToString(PropertyItem[] items)
    {
        using (MemoryStream memoryStream = new MemoryStream(1024))
        {
            this._dcs.WriteObject((Stream)memoryStream, (object)items);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }

    public PropertyItem[] FromString(string xmlString)
    {
        using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            return (PropertyItem[])this._dcs.ReadObject((Stream)memoryStream);
    }
}
