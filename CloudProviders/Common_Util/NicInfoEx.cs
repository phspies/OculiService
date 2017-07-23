using System;
using System.Runtime.Serialization;

[Serializable]
public class NicInfoEx : NicInfo
{
    [DataMember]
    public string[] IPDNSAddress;
    [DataMember]
    public string[] IPMask;
    [DataMember]
    public string[] IPGateway;
}
