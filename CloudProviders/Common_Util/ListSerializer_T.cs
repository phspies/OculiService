using System.Collections.Generic;

public class ListSerializer<T> : ObjectSerializer<T[]>
{
    private List<T> Data;

    public ListSerializer(string[] storeName)
      : base(storeName)
    {
        this.Data = new List<T>();
        this.Load();
    }

    public void Add(T obj)
    {
        lock (this)
        {
            this.Data.Add(obj);
            this.Save();
        }
    }

    public void Remove(T obj)
    {
        lock (this)
        {
            this.Data.Remove(obj);
            this.Save();
        }
    }

    public bool Contains(T obj)
    {
        lock (this)
            return this.Data.Contains(obj);
    }

    public T[] ToArray()
    {
        lock (this)
            return this.Data.ToArray();
    }

    public virtual void Load()
    {
        lock (this)
        {
            foreach (T item_0 in base.Load())
                this.Data.Add(item_0);
        }
    }

    public virtual void Save()
    {
        lock (this)
            this.Save(this.Data.ToArray());
    }
}
