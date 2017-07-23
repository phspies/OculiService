using System;
using System.Runtime.Serialization;

[DataContract]
[Serializable]
public class NicInfo
{
    [DataMember]
    public string[] IPs;
    [DataMember]
    public string PNPInstanceId;
    [DataMember]
    public string TcpIpServiceUuid;
    [DataMember]
    public string VirtualNetwork;
    [DataMember]
    public string FriendlyName;
    [DataMember]
    public int BusNumber;
    [DataMember]
    public int DeviceNumber;
    [DataMember]
    public int Index;

    public NicInfo()
    {
        this.BusNumber = 0;
        this.DeviceNumber = 0;
        this.Index = 9999;
    }
}
