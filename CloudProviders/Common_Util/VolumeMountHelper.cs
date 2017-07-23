using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public class VolumeMountHelper : IDisposable
{
    private string _mountDir;
    private string _mountLocation;
    private bool _mounted;

    public VolumeMountHelper(string mountDir)
    {
        this._mountDir = mountDir;
    }

    public string MountVolumeBySignature(byte[] volumeSignature)
    {
        string empty = string.Empty;
        if (!CVolume.FindVolumeSymboliclinkBySignature(volumeSignature, ref empty))
        {
            if (CVolume.FindVolumeDosDeviceNameBySignature(volumeSignature, ref empty))
            {
                string[] strArray = empty.Split('\\');
                if (strArray != null && strArray.Length >= 3)
                    return strArray[2];
            }
            throw new OculiServiceException(0, string.Format("Unable to find a volume mounted with a signature matching:'{0}'.", (object)VolumeMountHelper.PrintbyteArray(volumeSignature)));
        }
        string volumeName = empty.Replace("??", "\\?");
        CVolume _volumeObj = new CVolume(volumeName + "\\");
        string str = empty.Substring(11, 36);
        this._mountLocation = this._mountDir.EndsWith("\\") ? this._mountDir + str : this._mountDir + "\\" + str;
        CUtils.Retry(5, 5000, (CUtils.Workload)(() =>
       {
           if (!_volumeObj.MountDrive(this._mountLocation))
               throw new OculiServiceException(0, string.Format("Error mounting volume {0} to location {1}. Error = {2}", (object)volumeName, (object)this._mountLocation, (object)Marshal.GetLastWin32Error()));
       }));
        this._mounted = true;
        return this._mountLocation;
    }

    public void Dispose()
    {
        if (!this._mounted || !CVolume.UnmountDrive(this._mountLocation))
            return;
        this._mounted = false;
        if (!Directory.Exists(this._mountLocation))
            return;
        Directory.Delete(this._mountLocation);
    }

    private static string PrintbyteArray(byte[] data)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in data)
            stringBuilder.Append(num.ToString("x2") + " ");
        return stringBuilder.ToString();
    }
}
