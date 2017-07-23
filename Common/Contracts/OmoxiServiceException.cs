using System;
using System.Runtime.Serialization;

[Serializable]
public class OculiServiceServiceException : ApplicationException, ISerializable
{
    private int _Id;

    public int Id
    {
        get
        {
            return this._Id;
        }
    }

    public OculiServiceServiceException(int id, string msg) : base(msg)
    {
        this._Id = id;
    }

    public OculiServiceServiceException(int id, string msg, Exception inner) : base(msg, inner)
    {
        this._Id = id;
    }

    protected OculiServiceServiceException(SerializationInfo info, StreamingContext ctx) : base(info, ctx)
    {
        this._Id = info.GetInt32("_Id");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
        base.GetObjectData(info, ctx);
        info.AddValue("_Id", this._Id);
    }
}
