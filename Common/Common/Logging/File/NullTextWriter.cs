﻿using System.IO;
using System.Text;

namespace OculiService.Common.Logging.File
{
  public sealed class NullTextWriter : TextWriter
  {
    public override Encoding Encoding
    {
      get
      {
        return Encoding.Default;
      }
    }

    public override void Write(string value)
    {
    }

    public override void Write(char[] buffer, int index, int count)
    {
    }

    public override void WriteLine()
    {
    }

    public override void WriteLine(object value)
    {
    }

    public override void WriteLine(string value)
    {
    }
  }
}
