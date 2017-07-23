using System;
using System.Runtime.Serialization;

[Serializable]
public class EsxException : ApplicationException, ISerializable
{
    public bool IsErrorRecoverable;

    public string ConsoleDetails
    {
        get
        {
            string str = "";
            if (this.InnerException != null)
            {
                for (Exception innerException = this.InnerException; innerException != null; innerException = innerException.InnerException)
                    str = str + innerException.Message + "\r\n\r\n";
            }
            return str;
        }
    }

    public string FullText
    {
        get
        {
            string str = "";
            Exception exception1 = (Exception)this;
            Exception exception2 = exception1;
            for (; exception1 != null; exception1 = exception1.InnerException)
            {
                str = str + exception1.Message + "\r\n\r\n";
                exception2 = exception1;
            }
            return str + exception2.StackTrace;
        }
    }

    public EsxException(string msg, bool isErrorRecoverable)
      : base(msg)
    {
        this.IsErrorRecoverable = isErrorRecoverable;
    }

    protected EsxException(SerializationInfo info, StreamingContext ctx)
      : base(info, ctx)
    {
        this.IsErrorRecoverable = true;
    }

    public EsxException(string msg, Exception ex, bool isErrorRecoverable)
      : base(msg, ex)
    {
        this.IsErrorRecoverable = isErrorRecoverable;
    }
}
