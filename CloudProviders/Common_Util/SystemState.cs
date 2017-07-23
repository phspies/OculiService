using OculiService.Core.Contract;
using System.Globalization;

public class SystemState
{
    public string ServerName;
    public OperatingSystemInfo OsInfo;
    public int OSProductSuite;
    public string SystemPath;
    public string SystemVolume;
    public string ProgramFilesPath;
    public long RAM;
    public int CPUs;
    public VolumeInfo[] Volumes;
    public ServerNicInfo[] NetworkAdapters;
    public string HalInternalName;
    public string HalVersion;

    public bool IsHALTypeCompatble(string targetHALType)
    {
        bool flag = false;
        if (string.Compare(this.HalInternalName, targetHALType, true, CultureInfo.InvariantCulture) == 0)
        {
            flag = true;
        }
        else
        {
            string lower = this.HalInternalName.ToLower();
            if (!(lower == "halapic.dll"))
            {
                if (!(lower == "halmps.dll"))
                {
                    if (!(lower == "halaacpi.dll"))
                    {
                        if (lower == "halmacpi.dll")
                            flag = string.Compare(targetHALType, "Halaacpi.dll", true, CultureInfo.InvariantCulture) == 0;
                    }
                    else
                        flag = string.Compare(targetHALType, "Halmacpi.dll", true, CultureInfo.InvariantCulture) == 0;
                }
                else
                    flag = string.Compare(targetHALType, "Halapic.dll", true, CultureInfo.InvariantCulture) == 0;
            }
            else
                flag = string.Compare(targetHALType, "Halmps.dll", true, CultureInfo.InvariantCulture) == 0;
        }
        return flag;
    }
}
