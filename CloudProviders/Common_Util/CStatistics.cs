using System;
using System.Collections.Generic;
using System.Threading;

public class CStatistics : IStatistics
{
    private ReaderWriterLock Lock;
    private Dictionary<string, object> Data;

    public CStatistics()
    {
        this.Data = new Dictionary<string, object>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);
        this.Lock = new ReaderWriterLock();
    }

    public object GetStatistics(string name)
    {
        object obj;
        try
        {
            this.Lock.AcquireReaderLock(-1);
            if (!this.Data.TryGetValue(name, out obj))
                obj = (object)null;
        }
        finally
        {
            this.Lock.ReleaseReaderLock();
        }
        if (obj is OnDemandEvaluation)
            obj = ((OnDemandEvaluation)obj)();
        return obj;
    }

    public object[] GetStatistics(string[] names)
    {
        if (names == null)
            return (object[])null;
        object[] objArray = new object[names.Length];
        try
        {
            this.Lock.AcquireReaderLock(-1);
            for (int index = 0; index < names.Length; ++index)
            {
                if (!this.Data.TryGetValue(names[index], out objArray[index]))
                    objArray[index] = (object)null;
            }
        }
        finally
        {
            this.Lock.ReleaseReaderLock();
        }
        for (int index = 0; index < objArray.Length; ++index)
        {
            if (objArray[index] is OnDemandEvaluation)
                objArray[index] = ((OnDemandEvaluation)objArray[index])();
        }
        return objArray;
    }

    public void SetStatistics(string name, object value)
    {
        try
        {
            this.Lock.AcquireWriterLock(-1);
            if (this.Data.ContainsKey(name))
                this.Data[name] = value;
            else
                this.Data.Add(name, value);
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public void SetStatistics(string[] names, object[] values)
    {
        if (names == null || values == null)
            return;
        if (names.Length != values.Length)
            return;
        try
        {
            this.Lock.AcquireWriterLock(-1);
            for (int index = 0; index < names.Length; ++index)
            {
                if (this.Data.ContainsKey(names[index]))
                    this.Data[names[index]] = values[index];
                else
                    this.Data.Add(names[index], values[index]);
            }
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }

    public void SetStatistics(Dictionary<string, object> items)
    {
        if (items == null)
            return;
        try
        {
            this.Lock.AcquireWriterLock(-1);
            foreach (string key in items.Keys)
            {
                if (this.Data.ContainsKey(key))
                    this.Data[key] = items[key];
                else
                    this.Data.Add(key, items[key]);
            }
        }
        finally
        {
            this.Lock.ReleaseWriterLock();
        }
    }
}
