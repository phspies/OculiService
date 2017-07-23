using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

public class CabUtils
{
    private string _cabFile;

    public CabUtils(string cabFile)
    {
        this._cabFile = cabFile;
    }

    public bool Extract(string file, string destFolder)
    {
        char[] chArray = new char[2] { '/', '\\' };
        destFolder = destFolder.TrimEnd(chArray);
        string path = destFolder + "\\" + file;
        if (File.Exists(path))
            File.Delete(path);
        Process.Start("extrac32", string.Format((IFormatProvider)CultureInfo.InvariantCulture, "/Y /E /L \"{0}\" \"{1}\" \"{2}\"", (object)destFolder, (object)this._cabFile, (object)file)).WaitForExit();
        return File.Exists(path);
    }
}
