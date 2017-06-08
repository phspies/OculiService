using System;using System.Runtime.Serialization;
using System.Text;

namespace OculiService.Common.ExceptionHandling
{
  [Serializable]
  public class RollbackException : Exception
  {
    public Exception Original { get; private set; }

    public Exception FromRollback { get; private set; }

    public RollbackException()
    {
    }

    public RollbackException(Exception original, Exception fromRollback)
      : this("A rollback operation failed", original, fromRollback)
    {
    }

    public RollbackException(string message, Exception original, Exception fromRollback)
      : base(message, original)
    {
      this.Original = original;
      this.FromRollback = fromRollback;
    }

    protected RollbackException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Original = (Exception) info.GetValue("OriginalEx", typeof (Exception));
      this.FromRollback = (Exception) info.GetValue("FromRollbackEx", typeof (Exception));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("OriginalEx", (object) this.Original);
      info.AddValue("FromRollbackEx", (object) this.FromRollback);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(base.ToString());
      if (this.Original != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.Append("---> (Original exception) ");
        stringBuilder.Append(this.Original.ToString());
        stringBuilder.AppendLine("<---");
      }
      if (this.FromRollback != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.Append("---> (Rollback exception) ");
        stringBuilder.Append(this.FromRollback.ToString());
        stringBuilder.AppendLine("<---");
      }
      return stringBuilder.ToString();
    }
  }
}
