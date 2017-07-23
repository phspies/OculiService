using System;
using System.Runtime.Serialization;

public class JobNicInfo
{
    public ServerNicInfo[] SourceNics;
    public ServerNicInfo[] Nics;
    public bool WANFailoverEnabled;

    public JobNicInfo()
    {
        this.SourceNics = new ServerNicInfo[0];
        this.Nics = new ServerNicInfo[0];
    }

    public void Reverse()
    {
        ServerNicInfo[] sourceNics = this.SourceNics;
        this.SourceNics = this.Nics;
        this.Nics = sourceNics;
    }
}
